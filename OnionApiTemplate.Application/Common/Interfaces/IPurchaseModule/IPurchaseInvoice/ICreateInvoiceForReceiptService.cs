using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseInvoice
{
    public interface ICreateInvoiceForReceiptService
    {
        Task<PurchaseInvoice> CreateInvoiceForReceiptAsync(CreatePurchaseInvoiceDto dto, string createdBy, CancellationToken cancellationToken = default);
    }
}
