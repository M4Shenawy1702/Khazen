using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Create
{
    public record CreateInvoiceForReceiptCommand(CreatePurchaseInvoiceDto Dto, string CurrentUserId) : IRequest<PurchaseInvoiceDto>;
}
