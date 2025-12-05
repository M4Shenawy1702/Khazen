namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Delete
{
    public record DeletePurchaseInvoiceCommand(Guid Id, string ModifiedBy, byte[] RowVersion) : IRequest<bool>;
}
