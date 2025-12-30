namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Reverse
{
    public record ReversePurchaseInvoiceCommand(Guid Id, string CurrentUserId, byte[] RowVersion) : IRequest<bool>;
}
