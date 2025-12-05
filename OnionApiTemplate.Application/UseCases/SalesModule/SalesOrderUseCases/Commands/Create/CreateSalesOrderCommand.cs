using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Create
{
    public record CreateSalesOrderCommand(CreateSalesOrderDto Dto, string CreatedBy) : IRequest<SalesOrderDto>;
}
