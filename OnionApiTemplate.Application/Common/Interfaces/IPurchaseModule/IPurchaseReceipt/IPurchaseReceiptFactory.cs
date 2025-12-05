using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Create;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt
{
    public interface IPurchaseReceiptFactory
    {
        PurchaseReceipt CreateReceipt(
            PurchaseOrder order,
            Warehouse warehouse,
            string CreatedBy,
           CreatePurchaseReceiptCommand command
        );
    }
}
