namespace Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Delete
{
    public record DeletePurchaseReceiptCommand(Guid Id, string CurrentUserId, byte[] RowVersion) : IRequest<bool>;
}
