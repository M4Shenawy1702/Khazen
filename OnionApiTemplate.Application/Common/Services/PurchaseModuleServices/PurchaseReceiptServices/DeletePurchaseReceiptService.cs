namespace Khazen.Application.Common.Services.PurchaseModuleServices.PurchaseReceiptServices
{
    using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt;
    using Khazen.Application.Specification.InventoryModule.WareHouseSpesifications;
    using Khazen.Domain.Entities.InventoryModule;
    using Khazen.Domain.Entities.PurchaseModule;
    using Khazen.Domain.Exceptions;
    using Microsoft.Extensions.Logging;


    public class DeletePurchaseReceiptService(
        IUnitOfWork unitOfWork,
        ILogger<DeletePurchaseReceiptService> logger
    ) : IDeletePurchaseReceiptService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<DeletePurchaseReceiptService> _logger = logger;

        public async Task DeleteReceiptAsync(PurchaseReceipt receipt, string modifiedBy, byte[]? rowVersion, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Setting RowVersion for PurchaseReceipt {ReceiptId}", receipt.Id);
            receipt.SetRowVersion(rowVersion);

            _logger.LogDebug("Adjusting warehouse stock for PurchaseReceipt {ReceiptId}", receipt.Id);
            await AdjustWarehouseStockAsync(receipt, cancellationToken);

            var receiptRepo = _unitOfWork.GetRepository<PurchaseReceipt, Guid>();
            _logger.LogDebug("Deleting PurchaseReceipt {ReceiptId}", receipt.Id);
            receipt.MarkAsDeleted(modifiedBy);
            receiptRepo.Update(receipt);
        }

        private async Task AdjustWarehouseStockAsync(PurchaseReceipt receipt, CancellationToken cancellationToken = default)
        {
            var warehouseProductRepo = _unitOfWork.GetRepository<WarehouseProduct, int>();
            var productIds = receipt.Items.Select(i => i.ProductId).ToList();
            var existingWarehouseProducts = await warehouseProductRepo.GetAllAsync(
                new GetProductsByWarehouseSpec(receipt.WarehouseId, productIds), cancellationToken);
            var existingProductsDict = existingWarehouseProducts.ToDictionary(x => x.ProductId, x => x);

            foreach (var item in receipt.Items)
            {
                if (!existingProductsDict.TryGetValue(item.ProductId, out var warehouseProduct))
                {
                    _logger.LogError("Product {ProductId} not found in Warehouse {WarehouseId}", item.ProductId, receipt.WarehouseId);
                    throw new BadRequestException($"Warehouse stock inconsistency: Product {item.ProductId} not found.");
                }

                if (warehouseProduct.QuantityInStock < item.ReceivedQuantity)
                {
                    _logger.LogWarning("Cannot delete Receipt {ReceiptId}. Stock for Product {ProductId} would go negative.", receipt.Id, item.ProductId);
                    throw new BadRequestException($"Cannot delete receipt. Stock for product {item.ProductId} would go negative.");
                }

                warehouseProduct.QuantityInStock -= item.ReceivedQuantity;
                warehouseProductRepo.Update(warehouseProduct);

                _logger.LogDebug("Adjusted stock for Product {ProductId}. New Quantity: {Quantity}", item.ProductId, warehouseProduct.QuantityInStock);
            }
        }
    }

}
