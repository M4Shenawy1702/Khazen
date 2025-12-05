using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDots;
using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Create
{
    public record CreatePurchaseOrderCommand(CreatePurchaseOrderDto Dto, string CreatedBy) : IRequest<PurchaseOrderDto>;
}
