using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDots;
using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Update
{
    public record UpdatePurchaseOrderCommand(Guid Id, UpdatePurchaseOrderDto Dto, string ModifiedBy) : IRequest<PurchaseOrderDto>;

}
