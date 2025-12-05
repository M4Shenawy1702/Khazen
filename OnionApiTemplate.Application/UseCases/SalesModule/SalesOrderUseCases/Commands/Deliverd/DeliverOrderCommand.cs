using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Deliverd
{
    public record DeliverOrderCommand(Guid Id, byte[] RowVersion, string DeliveredBy) : IRequest<SalesOrderDto>;
}
