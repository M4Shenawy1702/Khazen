namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Delete
{
    public record ToggleProductCommand(Guid Id, string CurrentUserId) : IRequest<bool>;
}
