using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Queries.GetById
{
    public record GetPurchaseInvoiceByIdQuery(Guid Id) : IRequest<PurchaseInvoiceDetailsDto>;
}
