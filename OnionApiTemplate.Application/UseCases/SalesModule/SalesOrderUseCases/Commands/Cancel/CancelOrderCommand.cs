using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Cancel
{
    public record CancelOrderCommand(Guid Id, byte[] RowVersion, string CanceledBy) : IRequest<SalesOrderDto>;
}
