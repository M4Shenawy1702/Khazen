using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseInvoice
{
    public interface IPurchaseInvoiceStockCostService
    {
        Task UpdateStockAndCostAsync(PurchaseInvoice invoice, Guid warehouseId, CancellationToken cancellationToken = default);
    }
}
