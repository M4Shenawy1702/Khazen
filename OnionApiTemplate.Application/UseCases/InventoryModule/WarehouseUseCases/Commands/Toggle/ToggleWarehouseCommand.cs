namespace Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Delete
{
    public record ToggleWarehouseCommand(Guid Id, string ModifiedBy) : IRequest<bool>;
}
