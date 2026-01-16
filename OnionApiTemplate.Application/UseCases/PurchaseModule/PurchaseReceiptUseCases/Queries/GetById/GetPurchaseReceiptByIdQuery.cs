using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Queries.GetById
{
    public record GetPurchaseReceiptByIdQuery(Guid Id) : IRequest<PurchaseReceiptDetailsDto>;
}
