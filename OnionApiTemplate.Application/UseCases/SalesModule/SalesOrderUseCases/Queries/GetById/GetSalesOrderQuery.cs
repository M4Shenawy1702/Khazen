using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Queries.GetById
{
    public record GetSalesOrderQuery(Guid Id) : IRequest<SalesOrderDetailsDto>;
}
