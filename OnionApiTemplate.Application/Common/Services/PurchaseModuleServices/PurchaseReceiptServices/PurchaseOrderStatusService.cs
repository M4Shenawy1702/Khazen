using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Services.PurchaseModuleServices.PurchaseReceiptServices
{
    public class PurchaseOrderStatusService : IPurchaseOrderStatusService
    {
        public void UpdateStatus(PurchaseOrder order, PurchaseReceipt receipt)
        {
            var totalOrdered = order.Items.Sum(i => i.Quantity);
            var totalReceivedBefore = order.PurchaseReceipts.Sum(r => r.Items.Sum(i => i.ReceivedQuantity));
            var totalReceivedNow = totalReceivedBefore + receipt.Items.Sum(i => i.ReceivedQuantity);

            order.Status = totalReceivedNow switch
            {
                0 => PurchaseOrderStatus.Pending,
                var x when x >= totalOrdered => PurchaseOrderStatus.FullyReceived,
                _ => PurchaseOrderStatus.PartiallyReceived
            };
        }
    }
}
