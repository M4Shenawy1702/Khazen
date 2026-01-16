using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Update;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt
{
    public interface IPurchaseReceiptUpdateService
    {
        Task UpdateReceiptAsync(UpdatePurchaseReceiptCommand request, PurchaseReceipt receipt, PurchaseOrder order, CancellationToken cancellationToken = default);
    }
}
