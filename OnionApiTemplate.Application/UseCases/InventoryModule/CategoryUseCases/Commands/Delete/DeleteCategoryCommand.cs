namespace Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Delete
{
    public record DeleteCategoryCommand(Guid Id) : IRequest<bool>;
}
