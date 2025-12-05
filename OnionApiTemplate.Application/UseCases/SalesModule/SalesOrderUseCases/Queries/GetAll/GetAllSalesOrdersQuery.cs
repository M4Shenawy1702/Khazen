using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Queries.GetAll
{
    public record GetAllSalesOrdersQuery(SalesOrdersQueryParameters queryParameters) : IRequest<PaginatedResult<SalesOrderDto>>;
}
