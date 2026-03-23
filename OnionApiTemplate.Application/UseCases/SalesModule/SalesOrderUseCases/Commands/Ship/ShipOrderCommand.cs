using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Ship
{
    public record ShipOrderCommand(Guid Id, ShipOrderDto Dto, byte[] RowVersion, string CurrentUserId) : IRequest<SalesOrderDto>;
}
