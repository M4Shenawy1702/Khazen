using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Update
{
    public record UpdateSalesOrderCommand(Guid Id, UpdateSalesOrderDto Dto, string ModifiedBy, byte[] RowVersion) : IRequest<SalesOrderDto>;
}
