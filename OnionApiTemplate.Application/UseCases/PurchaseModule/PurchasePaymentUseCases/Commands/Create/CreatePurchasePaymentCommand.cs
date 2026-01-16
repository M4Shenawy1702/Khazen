using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;

namespace Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Commands.Create
{
    public record CreatePurchasePaymentCommand(CreatePurchasePaymentDto Dto, string CurrentUserId) : IRequest<PurchasePaymentDto>;
}
