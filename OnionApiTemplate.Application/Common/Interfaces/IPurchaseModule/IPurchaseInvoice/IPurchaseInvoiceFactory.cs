using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseInvoice
{
    public interface IInvoiceFactoryService
    {
        Task<PurchaseInvoice> CreateInvoiceAsync(PurchaseReceipt receipt, CreatePurchaseInvoiceDto Dto, string userId, CancellationToken cancellationToken = default);
    }
}
