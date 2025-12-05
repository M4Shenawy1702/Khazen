using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Confirm
{
    public record ConfirmOrderCommand(Guid Id, byte[] RowVersion, string ConfirmedBy) : IRequest<SalesOrderDto>;
}
