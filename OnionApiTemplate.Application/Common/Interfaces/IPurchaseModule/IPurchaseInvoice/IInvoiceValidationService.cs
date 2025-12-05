using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Create;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseInvoice
{
    public interface IInvoiceValidationService
    {
        Task<PurchaseReceipt> ValidateAndGetReceiptAsync(CreateInvoiceForReceiptCommand request, CancellationToken cancellationToken);
    }

}
