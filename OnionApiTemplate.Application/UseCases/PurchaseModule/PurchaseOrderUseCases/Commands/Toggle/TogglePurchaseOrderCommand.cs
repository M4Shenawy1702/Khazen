namespace Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Delete
{
    public record TogglePurchaseOrderCommand(Guid Id, string ModifiedBy, byte[] RowVersion) : IRequest<bool>;
}
