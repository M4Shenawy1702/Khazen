using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;

namespace Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Commands.Delete
{
    public record ReversePurchasePaymentCommand(Guid Id,
                                                byte[] RowVersion,
                                                string CurrentUserId) : IRequest<PurchasePaymentDto>;
}
