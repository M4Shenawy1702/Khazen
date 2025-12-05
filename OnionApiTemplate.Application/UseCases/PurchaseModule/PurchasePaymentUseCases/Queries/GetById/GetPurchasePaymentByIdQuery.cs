using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;

namespace Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Queries.GetById
{
    public record GetPurchasePaymentByIdQuery(Guid Id) : IRequest<PurchasePaymentDetailsDto>;
}
