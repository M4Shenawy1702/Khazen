using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Queries.GetAll
{
    public record GetAllPurchaseInvoicesQuery(PurchaseInvoiceQueryParameters QueryParameters) : IRequest<PaginatedResult<PurchaseInvoiceDto>>;
}
