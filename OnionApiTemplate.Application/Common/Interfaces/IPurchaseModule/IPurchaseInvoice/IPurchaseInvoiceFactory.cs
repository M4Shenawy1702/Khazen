using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Create;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseInvoice
{
    public interface IInvoiceFactoryService
    {
        Task<PurchaseInvoice> CreateInvoiceAsync(PurchaseReceipt receipt, CreateInvoiceForReceiptCommand command, CancellationToken cancellationToken = default);
    }
}
