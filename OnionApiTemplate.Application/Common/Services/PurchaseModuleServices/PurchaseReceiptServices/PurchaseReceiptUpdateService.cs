using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt;
using Khazen.Application.Specification.InventoryModule.WareHouseSpesifications;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Update;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services.PurchaseModuleServices.PurchaseReceiptServices
{

    public class PurchaseReceiptUpdateService : IPurchaseReceiptUpdateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PurchaseReceiptUpdateService> _logger;

        public PurchaseReceiptUpdateService(IUnitOfWork unitOfWork, ILogger<PurchaseReceiptUpdateService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task UpdateReceiptAsync(
            UpdatePurchaseReceiptCommand request,
            PurchaseReceipt receipt,
            PurchaseOrder order,
            CancellationToken cancellationToken = default
        )
        {
            _logger.LogInformation("Starting update for Purchase Receipt ID {PurchaseReceiptId}", receipt.Id);

            receipt.SetRowVersion(request.Dto.RowVersion);

            await RollbackWarehouseStockAsync(receipt, cancellationToken);
            _logger.LogInformation("Rolled back warehouse stock for Receipt ID {PurchaseReceiptId}", receipt.Id);

            UpdateReceiptItems(request, receipt, order);
            _logger.LogInformation("Updated receipt items for Receipt ID {PurchaseReceiptId}", receipt.Id);

            ValidateProducts(order, receipt);
            ValidateQuantities(order, receipt);
            _logger.LogInformation("Validated products and quantities for Receipt ID {PurchaseReceiptId}", receipt.Id);

            await AdjustWarehouseStockAsync(receipt, cancellationToken);
            _logger.LogInformation("Adjusted warehouse stock for Receipt ID {PurchaseReceiptId}", receipt.Id);

            order.UpdateStatusBasedOnReceipts();
            _logger.LogInformation("Updated Purchase Order status for Order ID {PurchaseOrderId}", order.Id);
        }


        private static void UpdateReceiptItems(UpdatePurchaseReceiptCommand request, PurchaseReceipt receipt, PurchaseOrder purchaseOrder)
        {
            if (request.Dto.Items == null || request.Dto.Items.Count == 0)
                throw new BadRequestException("Receipt must have at least one item.");

            var existingItems = receipt.Items.ToDictionary(i => i.ProductId);
            var incomingItems = request.Dto.Items!.ToDictionary(i => i.ProductId);

            foreach (var itemDto in request.Dto.Items)
            {
                if (!purchaseOrder.Items.Any(p => p.ProductId == itemDto.ProductId))
                    throw new BadRequestException($"Product {itemDto.ProductId} does not belong to PurchaseOrder {purchaseOrder.Id}");

                if (itemDto.Quantity <= 0)
                    throw new BadRequestException($"Quantity for product {itemDto.ProductId} must be greater than zero.");

                if (existingItems.TryGetValue(itemDto.ProductId, out var existingItem))
                {
                    existingItem.ReceivedQuantity = itemDto.Quantity;
                }
                else
                {
                    receipt.Items.Add(new PurchaseReceiptItem
                    {
                        PurchaseReceiptId = receipt.Id,
                        ProductId = itemDto.ProductId,
                        ReceivedQuantity = itemDto.Quantity
                    });
                }
            }

            foreach (var item in receipt.Items.Where(i => !incomingItems.ContainsKey(i.ProductId)).ToList())
                receipt.Items.Remove(item);
        }


        private static void ValidateProducts(PurchaseOrder purchaseOrder, PurchaseReceipt receipt)
        {
            if (receipt.Items.Count == 0)
                throw new BadRequestException("Receipt must have at least one item.");

            var orderProducts = purchaseOrder.Items.Select(p => p.ProductId).ToHashSet();

            foreach (var item in receipt.Items)
                if (!orderProducts.Contains(item.ProductId))
                    throw new BadRequestException($"Product {item.ProductId} is not part of Purchase Order {purchaseOrder.Id}");
        }


        private static void ValidateQuantities(PurchaseOrder purchaseOrder, PurchaseReceipt receipt)
        {
            var orderedQty = purchaseOrder.Items.ToDictionary(i => i.ProductId, i => i.Quantity);

            var receivedBefore = purchaseOrder.PurchaseReceipts
                .Where(r => r.Id != receipt.Id)
                .SelectMany(r => r.Items)
                .GroupBy(i => i.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.ReceivedQuantity));

            foreach (var item in receipt.Items)
            {
                orderedQty.TryGetValue(item.ProductId, out var ordered);
                var alreadyReceived = receivedBefore.GetValueOrDefault(item.ProductId, 0);

                if (item.ReceivedQuantity + alreadyReceived > ordered)
                    throw new BadRequestException(
                        $"Received quantity for product {item.ProductId} exceeds ordered amount. (Ordered: {ordered}, Received: {alreadyReceived}, This: {item.ReceivedQuantity})");
            }
        }


        private async Task RollbackWarehouseStockAsync(PurchaseReceipt receipt, CancellationToken cancellationToken)
        {
            var warehouseProductRepo = _unitOfWork.GetRepository<WarehouseProduct, int>();
            var productIds = receipt.Items.Select(i => i.ProductId).ToList();

            var warehouseProducts = await warehouseProductRepo.GetAllAsync(
                new GetProductsByWarehouseSpec(receipt.WarehouseId, productIds),
                cancellationToken
            );

            foreach (var item in receipt.Items)
            {
                var wp = warehouseProducts.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (wp != null)
                {
                    wp.QuantityInStock = Math.Max(0, wp.QuantityInStock - item.ReceivedQuantity);
                    warehouseProductRepo.Update(wp);
                    _logger.LogInformation("Rolled back stock for Product ID {ProductId} by {Quantity}", item.ProductId, item.ReceivedQuantity);
                }
            }
        }

        private async Task AdjustWarehouseStockAsync(PurchaseReceipt receipt, CancellationToken cancellationToken)
        {
            var warehouseProductRepo = _unitOfWork.GetRepository<WarehouseProduct, int>();
            var productIds = receipt.Items.Select(i => i.ProductId).ToList();

            var warehouseProducts = await warehouseProductRepo.GetAllAsync(
                new GetProductsByWarehouseSpec(receipt.WarehouseId, productIds),
                cancellationToken
            );

            var dict = warehouseProducts.ToDictionary(x => x.ProductId);

            foreach (var item in receipt.Items)
            {
                if (dict.TryGetValue(item.ProductId, out var wp))
                {
                    wp.QuantityInStock += item.ReceivedQuantity;
                    warehouseProductRepo.Update(wp);
                    _logger.LogInformation("Adjusted stock for Product ID {ProductId} by {Quantity}", item.ProductId, item.ReceivedQuantity);
                }
                else
                {
                    await warehouseProductRepo.AddAsync(new WarehouseProduct
                    {
                        WarehouseId = receipt.WarehouseId,
                        ProductId = item.ProductId,
                        QuantityInStock = item.ReceivedQuantity
                    }, cancellationToken);
                    _logger.LogInformation("Added new warehouse stock for Product ID {ProductId} with {Quantity}", item.ProductId, item.ReceivedQuantity);
                }
            }
        }
    }
}

