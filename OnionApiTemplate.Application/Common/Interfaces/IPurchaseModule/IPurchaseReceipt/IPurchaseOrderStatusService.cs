using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt
{
    public interface IPurchaseOrderStatusService
    {
        void UpdateStatus(PurchaseOrder order, PurchaseReceipt receipt);
    }
}
