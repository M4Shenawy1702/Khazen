using Khazen.Application.Common.Interfaces.SalesModule.ISalesOrderModule;
using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services.SalesOrderServices
{
    internal class SalesOrderService(ILogger<SalesOrderService> logger) : ISalesOrderService
    {
        private readonly ILogger<SalesOrderService> _logger = logger;

        public SalesOrder CreateSalesOreder(Customer customer, CreateSalesOrderDto dto, IEnumerable<WarehouseProduct> warehouseProducts, string createdBy)
        {
            _logger.LogInformation("Creating SalesOrder for CustomerId: {CustomerId} ", customer.Id);

            var order = new SalesOrder
            {
                CustomerId = customer.Id,
                EstimatedShipDate = dto.EstimatedShipDate,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                CreatedBy = createdBy,
                CustomerNameSnapshot = $"{customer.Name}-{customer.Address}"
            };

            _logger.LogDebug("Building product lookup for fast access...");

            var productLookup = warehouseProducts
                .GroupBy(w => w.ProductId)
                .ToDictionary(g => g.Key, g => g.ToList());

            _logger.LogDebug("Product lookup created with {Count} distinct products", productLookup.Count);

            foreach (var itemDto in dto.SalesOrderItems)
            {
                _logger.LogDebug("Processing SalesOrderItem for ProductId: {ProductId}, Quantity: {Quantity}", itemDto.ProductId, itemDto.Quantity);

                if (!productLookup.TryGetValue(itemDto.ProductId, out var wpList))
                {
                    _logger.LogError("No WarehouseProduct found for ProductId: {ProductId}", itemDto.ProductId);
                    throw new NotFoundException<WarehouseProduct>($"No stock found for product {itemDto.ProductId}");
                }

                var wp = wpList.First();
                var product = wp.Product;

                _logger.LogDebug(
                    "Adding item: Product: {ProductName}, UnitPrice: {Price}, TaxRate: {TaxRate}",
                    product.Name,
                    product.SellingPrice,
                    product.TaxRate
                );

                order.AddItem(new SalesOrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.SellingPrice,
                    DiscountType = itemDto.DiscountType,
                    DiscountValue = itemDto.DiscountValue,
                    TaxRate = product.TaxRate
                });
            }

            _logger.LogInformation(
                "SalesOrder created successfully with {TotalItems} items",
                order.Items.Count
            );

            return order;
        }
        public async Task<SalesOrder> UpdateSalesOrderAsync(
            Customer customer,
            UpdateSalesOrderDto dto,
            SalesOrder salesOrder,
            IEnumerable<WarehouseProduct> warehouseProducts,
            string modifiedBy)
        {
            _logger.LogInformation("Updating Sales Order {OrderId} by {User}", salesOrder.Id, modifiedBy);

            salesOrder.CustomerId = customer.Id;
            salesOrder.CustomerNameSnapshot = $"{customer.Name}-{customer.Address}";
            salesOrder.EstimatedShipDate = dto.EstimatedShipDate;
            salesOrder.DiscountType = dto.DiscountType;
            salesOrder.DiscountValue = dto.DiscountValue;
            salesOrder.ModifiedBy = modifiedBy;
            salesOrder.ModifiedAt = DateTime.UtcNow;

            var existingItems = salesOrder.Items.ToList();

            foreach (var oldItem in existingItems)
            {
                if (!dto.SalesOrderItems.Any(i => i.ProductId == oldItem.ProductId))
                {
                    _logger.LogDebug("Removing item ProductId {ProductId}", oldItem.ProductId);
                    salesOrder.Items.Remove(oldItem);
                }
            }

            foreach (var itemDto in dto.SalesOrderItems)
            {
                var existing = salesOrder.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);

                var wp = warehouseProducts.First(w => w.ProductId == itemDto.ProductId);
                var product = wp.Product;

                if (existing is null)
                {
                    _logger.LogDebug("Adding new item ProductId {ProductId}", itemDto.ProductId);

                    salesOrder.Items.Add(new SalesOrderItem
                    {
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        UnitPrice = product.SellingPrice,
                        DiscountType = itemDto.DiscountType,
                        DiscountValue = itemDto.DiscountValue,
                        TaxRate = product.TaxRate
                    });
                }
                else
                {
                    _logger.LogDebug("Updating existing item ProductId {ProductId}", itemDto.ProductId);

                    existing.Quantity = itemDto.Quantity;
                    existing.DiscountType = itemDto.DiscountType;
                    existing.DiscountValue = itemDto.DiscountValue;
                    existing.UnitPrice = product.SellingPrice;
                    existing.TaxRate = product.TaxRate;
                }
            }

            salesOrder.CalculateTotals();

            _logger.LogInformation("Sales Order {OrderId} updated successfully", salesOrder.Id);

            return salesOrder;
        }
    }
}
