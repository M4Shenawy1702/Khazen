using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;

namespace Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Commands.Delete
{
    public record DeletePurchasePaymentCommand(Guid Id, string ReversedBy) : IRequest<PurchasePaymentDto>;
}
