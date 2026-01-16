namespace Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Delete
{
    public record TogglePurchaseOrderCommand(Guid Id, string CurrentUserId, byte[] RowVersion) : IRequest<bool>;
}
