namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Delete
{
    public record DeletePurchaseInvoiceCommand(Guid Id, string CurrentUserId, byte[] RowVersion) : IRequest<bool>;
}
