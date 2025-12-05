using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchasePaymentServices
{
    public interface IPurchasePaymentDomainService
    {
        Task<PurchasePayment> CreatePaymentAsync(PurchaseInvoice invoice, CreatePurchasePaymentDto Dto, CancellationToken cancellationToken);
    }
}
