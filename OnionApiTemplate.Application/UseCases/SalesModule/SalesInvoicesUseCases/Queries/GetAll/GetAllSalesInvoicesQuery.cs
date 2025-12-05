using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Queries.GetAll
{
    public record GetAllSalesInvoicesQuery(SalesInvoicesQueryParameters QueryParameters) : IRequest<PaginatedResult<SalesInvoiceDto>>;
}
