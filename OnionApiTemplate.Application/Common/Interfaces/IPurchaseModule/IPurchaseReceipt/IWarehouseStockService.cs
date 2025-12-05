using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt
{
    public interface IWarehouseStockService
    {
        Task AdjustStockAsync(PurchaseReceipt receipt, CancellationToken cancellationToken);
    }
}
