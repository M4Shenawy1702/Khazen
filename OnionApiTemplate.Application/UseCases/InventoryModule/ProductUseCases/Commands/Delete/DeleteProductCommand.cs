namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Delete
{
    public record DeleteProductCommand(Guid Id) : IRequest<bool>;
}
