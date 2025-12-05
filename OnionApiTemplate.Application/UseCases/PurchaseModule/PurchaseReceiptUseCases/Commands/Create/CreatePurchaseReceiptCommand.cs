using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Create
{
    public record CreatePurchaseReceiptCommand(CreatePurchaseReceiptDto Dto, string CreatedBy)
        : IRequest<PurchaseReceiptDto>;

}
