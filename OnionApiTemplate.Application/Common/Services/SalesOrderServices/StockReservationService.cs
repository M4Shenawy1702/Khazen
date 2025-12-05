using Khazen.Application.Common.Interfaces.SalesModule.ISalesOrderModule;
using Khazen.Application.DOTs.SalesModule.SalesOrderItemDtos;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services.SalesOrderServices
{
    internal class StockReservationService(ILogger<StockReservationService> logger) : IStockReservationService
    {
        private readonly ILogger<StockReservationService> _logger = logger;

        public async Task ReserveStockAsync(
            IEnumerable<AddSalesOrderItemDto> items,
            IEnumerable<WarehouseProduct> warehouseProducts)
        {
            _logger.LogInformation("Starting stock reservation for {Count} items", items.Count());

            var productGroups = warehouseProducts
                .GroupBy(wp => wp.ProductId)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.WarehouseId).ToList());

            foreach (var item in items)
            {
                if (!productGroups.ContainsKey(item.ProductId))
                {
                    _logger.LogError("Stock not found for Product {ProductId}", item.ProductId);
                    throw new NotFoundException<WarehouseProduct>($"No stock found for product {item.ProductId}");
                }

                var stocks = productGroups[item.ProductId];

                int remaining = item.Quantity;

                foreach (var wp in stocks)
                {
                    if (remaining <= 0) break;

                    int available = wp.QuantityInStock - wp.ReservedQuantity;
                    int reserve = Math.Min(remaining, available);

                    wp.ReservedQuantity += reserve;
                    remaining -= reserve;

                    _logger.LogDebug(
                        "Reserved {Amount} units from Warehouse {WarehouseId} for Product {ProductId}. Remaining {Remaining}",
                        reserve, wp.WarehouseId, item.ProductId, remaining);
                }

                if (remaining > 0)
                {
                    _logger.LogError("Insufficient stock for Product {ProductId}. Needed {Needed}", item.ProductId, item.Quantity);
                    throw new BadRequestException($"Insufficient stock for product {item.ProductId}");
                }
            }

            _logger.LogInformation("Stock reservation completed successfully.");
        }
        public async Task ValidateReservedQuantitiesAsync(SalesOrder order, Dictionary<Guid, Product> productLookup)
        {
            foreach (var item in order.Items)
            {
                if (!productLookup.TryGetValue(item.ProductId, out var product))
                    throw new NotFoundException<Product>(item.ProductId);

                if (product.ReservedQuantity < item.Quantity)
                    throw new BadRequestException($"Reserved quantity for product {product.Name} is inconsistent");
            }
        }
        public async Task UnReserveStockAsync(SalesOrder salesOrder, IEnumerable<WarehouseProduct> warehouseProducts)
        {
            _logger.LogInformation("Starting UnReserveStock | OrderId: {OrderId}", salesOrder.Id);

            var lookup = warehouseProducts
                .GroupBy(w => w.ProductId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var item in salesOrder.Items)
            {
                if (!lookup.TryGetValue(item.ProductId, out var reservedStocks) || reservedStocks.Count == 0)
                {
                    _logger.LogWarning(
                        "No reserved stock found for product {ProductId} | OrderId: {OrderId}",
                        item.ProductId, salesOrder.Id);
                    throw new NotFoundException<WarehouseProduct>($"No reserved stock found for product {item.ProductId}");
                }

                int remainingToRelease = item.Quantity;
                _logger.LogInformation(
                    "Unreserving stock for product {ProductId} | Quantity: {Quantity} | OrderId: {OrderId}",
                    item.ProductId, item.Quantity, salesOrder.Id);

                foreach (var wp in reservedStocks.OrderBy(wp => wp.WarehouseId))
                {
                    if (remainingToRelease <= 0) break;

                    var release = Math.Min(remainingToRelease, wp.ReservedQuantity);

                    if (release <= 0) continue;

                    wp.ReservedQuantity -= release;
                    remainingToRelease -= release;

                    _logger.LogInformation(
                        "Released {ReleasedQuantity} units from warehouse {WarehouseId} | ProductId: {ProductId} | RemainingToRelease: {Remaining} | OrderId: {OrderId}",
                        release, wp.WarehouseId, item.ProductId, remainingToRelease, salesOrder.Id);
                }

                if (remainingToRelease > 0)
                {
                    _logger.LogWarning(
                        "Reserved stock insufficient for product {ProductId} | RemainingToRelease: {Remaining} | OrderId: {OrderId}",
                        item.ProductId, remainingToRelease, salesOrder.Id);
                    throw new BadRequestException($"Reserved stock insufficient for product {item.ProductId}");
                }
            }

            _logger.LogInformation("Completed UnReserveStock | OrderId: {OrderId}", salesOrder.Id);
        }
        public async Task UpdateReservedQuantitiesWhenOrderShipped(
         SalesOrder salesOrder,
         IEnumerable<WarehouseProduct> warehouseProducts)
        {
            _logger.LogInformation("Start updating reserved quantities for order {OrderId}", salesOrder.Id);

            var lookup = warehouseProducts
                .GroupBy(w => w.ProductId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var item in salesOrder.Items)
            {
                _logger.LogInformation("Processing product {ProductId} with quantity {Quantity}", item.ProductId, item.Quantity);

                if (!lookup.TryGetValue(item.ProductId, out var availableStocks) || availableStocks.Count == 0)
                {
                    _logger.LogWarning("No stock found for product {ProductId}", item.ProductId);
                    throw new NotFoundException<WarehouseProduct>($"No stock found for product {item.ProductId}");
                }

                var totalAvailable = availableStocks.Sum(wp => wp.QuantityInStock);
                _logger.LogInformation("Total available stock for product {ProductId}: {TotalAvailable}", item.ProductId, totalAvailable);

                if (totalAvailable < item.Quantity)
                {
                    _logger.LogWarning("Insufficient stock for product {ProductId}. Needed: {Needed}, Available: {Available}",
                        item.ProductId, item.Quantity, totalAvailable);
                    throw new BadRequestException($"Insufficient stock for product {item.ProductId} across all warehouses.");
                }

                int remainingToDeduct = item.Quantity;

                foreach (var wp in availableStocks.OrderBy(wp => wp.WarehouseId))
                {
                    if (remainingToDeduct <= 0) break;

                    var deduct = Math.Min(remainingToDeduct, wp.QuantityInStock);

                    if (wp.ReservedQuantity < deduct)
                    {
                        _logger.LogWarning("Reserved quantity not enough for product {ProductId} in warehouse {WarehouseId}. Required: {Required}, Reserved: {Reserved}",
                            item.ProductId, wp.WarehouseId, deduct, wp.ReservedQuantity);
                        throw new BadRequestException(
                            $"Reserved quantity not enough for product {item.ProductId} in warehouse {wp.WarehouseId}"
                        );
                    }

                    wp.ReservedQuantity -= deduct;
                    wp.QuantityInStock -= deduct;
                    remainingToDeduct -= deduct;

                    _logger.LogInformation("Deducted {Deduct} units from warehouse {WarehouseId} for product {ProductId}. Remaining to deduct: {Remaining}. Reserved left: {Reserved}, Stock left: {Stock}",
                        deduct, wp.WarehouseId, item.ProductId, remainingToDeduct, wp.ReservedQuantity, wp.QuantityInStock);
                }

                if (remainingToDeduct > 0)
                {
                    _logger.LogError("Failed to deduct full quantity for product {ProductId}. Remaining: {Remaining}", item.ProductId, remainingToDeduct);
                    throw new BadRequestException($"Failed to deduct full quantity for product {item.ProductId}");
                }
            }

            _logger.LogInformation("Finished updating reserved quantities for order {OrderId}", salesOrder.Id);
        }
        public void ReleaseOldReservations(
            SalesOrder oldOrder,
            IEnumerable<WarehouseProduct> warehouseProducts)
        {
            _logger.LogInformation("Releasing old reservations for Order {OrderId}", oldOrder.Id);

            foreach (var oldItem in oldOrder.Items)
            {
                var stockRecords = warehouseProducts
                    .Where(wp => wp.ProductId == oldItem.ProductId)
                    .OrderBy(wp => wp.WarehouseId)
                    .ToList();

                if (stockRecords.Count == 0)
                {
                    _logger.LogError("No stock found for Product {ProductId}", oldItem.ProductId);
                    throw new NotFoundException<WarehouseProduct>($"No stock found for Product {oldItem.ProductId}");
                }

                int remainingToRelease = oldItem.Quantity;

                foreach (var wp in stockRecords)
                {
                    if (remainingToRelease <= 0)
                        break;

                    int releaseAmount = Math.Min(remainingToRelease, wp.ReservedQuantity);

                    if (releaseAmount <= 0)
                    {
                        _logger.LogError("Reserved stock inconsistency for Product {ProductId}", oldItem.ProductId);
                        throw new BadRequestException($"Reserved stock inconsistency for Product {oldItem.ProductId}");
                    }

                    _logger.LogDebug(
                        "Releasing {Amount} units from Warehouse {WarehouseId} for Product {ProductId}",
                        releaseAmount, wp.WarehouseId, oldItem.ProductId);

                    wp.ReservedQuantity -= releaseAmount;
                    remainingToRelease -= releaseAmount;
                }
            }

            _logger.LogInformation("Old reservations released for Order {OrderId}", oldOrder.Id);
        }
    }
}
//public Task FinalizeReservationAsync(SalesOrder order, Dictionary<Guid, Product> productLookup)
//{
//    foreach (var item in order.Items)
//    {
//        var product = productLookup[item.ProductId];

//        if (product.QuantityInStock < item.Quantity)
//            throw new BadRequestException($"Insufficient stock while finalizing order for product {product.Name}");

//        // Deduct from stock
//        product.QuantityInStock -= item.Quantity;

//        // Remove reservation
//        product.ReservedQuantity -= item.Quantity;
//    }

//    return Task.CompletedTask;
//}

