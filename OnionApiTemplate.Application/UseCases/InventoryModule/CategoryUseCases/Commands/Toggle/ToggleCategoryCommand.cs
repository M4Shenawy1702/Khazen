namespace Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Delete
{
    public record ToggleCategoryCommand(Guid Id, string CurrentUserId) : IRequest<bool>;
}
