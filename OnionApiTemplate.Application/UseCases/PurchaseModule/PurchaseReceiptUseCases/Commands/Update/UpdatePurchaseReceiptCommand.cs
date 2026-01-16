using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Update
{
    public record UpdatePurchaseReceiptCommand(Guid Id, UpdatePurchaseReceiptDto Dto, string CurrentUserId)
        : IRequest<PurchaseReceiptDto>;
}
