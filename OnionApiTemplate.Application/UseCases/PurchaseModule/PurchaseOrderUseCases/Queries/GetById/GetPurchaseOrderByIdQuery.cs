using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Queries.GetById
{
    public record GetPurchaseOrderByIdQuery(Guid Id) : IRequest<PurchaseOrderDetailsDto>;

}
