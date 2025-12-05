using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderPaymentUseCases.Queries.GetAll
{
    public record GetAllSalesInvoicePaymentsQuery(SalesOrderPaymentQueryParameters QueryParameters)
        : IRequest<PaginatedResult<SalesInvoicePaymentDto>>;
}