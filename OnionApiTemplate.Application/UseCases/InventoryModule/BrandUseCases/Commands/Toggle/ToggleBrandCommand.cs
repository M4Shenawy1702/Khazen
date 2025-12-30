namespace Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Delete
{
    public record ToggleBrandCommand(Guid Id, string CurrentUserId) : IRequest<bool>;
}
