namespace Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Delete
{
    public record DeletePurchaseReceiptCommand(Guid Id, string ModifiedBy, byte[] RowVersion) : IRequest<bool>;
}
