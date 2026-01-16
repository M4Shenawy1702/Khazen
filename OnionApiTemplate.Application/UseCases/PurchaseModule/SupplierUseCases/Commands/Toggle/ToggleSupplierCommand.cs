namespace Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Delete
{
    public record ToggleSupplierCommand(Guid Id, string CurrentUserId) : IRequest<bool>;
}
