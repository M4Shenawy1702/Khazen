namespace Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Delete
{
    public record ToggleSupplierCommand(Guid Id, string ModifiedBy) : IRequest<bool>;
}
