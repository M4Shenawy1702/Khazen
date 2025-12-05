using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt;
using Khazen.Application.Specification.InventoryModule.WareHouseSpesifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Services.PurchaseModuleServices.PurchaseReceiptServices
{
    public class WarehouseStockService(IUnitOfWork unitOfWork) : IWarehouseStockService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task AdjustStockAsync(PurchaseReceipt receipt, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<WarehouseProduct, int>();

            var productIds = receipt.Items.Select(x => x.ProductId).ToList();
            var existing = await repo.GetAllAsync(
                new GetProductsByWarehouseSpec(receipt.WarehouseId, productIds),
                cancellationToken);

            var map = existing.ToDictionary(x => x.ProductId);

            foreach (var item in receipt.Items)
            {
                if (map.TryGetValue(item.ProductId, out var warehouseProduct))
                {
                    warehouseProduct.QuantityInStock += item.ReceivedQuantity;
                    repo.Update(warehouseProduct);
                }
                else
                {
                    await repo.AddAsync(new WarehouseProduct
                    {
                        WarehouseId = receipt.WarehouseId,
                        ProductId = item.ProductId,
                        QuantityInStock = item.ReceivedQuantity
                    }, cancellationToken);
                }
            }
        }
    }

}
