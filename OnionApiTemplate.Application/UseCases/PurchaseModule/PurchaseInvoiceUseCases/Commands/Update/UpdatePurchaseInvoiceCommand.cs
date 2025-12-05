using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Update
{
    public record UpdatePurchaseInvoiceCommand(Guid Id, UpdatePurchaseInvoiceDto Dto, string ModifiedBy)
        : IRequest<PurchaseInvoiceDto>;
}
