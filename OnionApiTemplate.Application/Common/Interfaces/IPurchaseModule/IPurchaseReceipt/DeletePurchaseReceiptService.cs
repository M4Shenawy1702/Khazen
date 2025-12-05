using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt
{
    public interface IDeletePurchaseReceiptService
    {
        Task DeleteReceiptAsync(PurchaseReceipt receipt, string modifiedBy, byte[]? rowVersion, CancellationToken cancellationToken = default);
    }
}
