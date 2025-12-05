using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchaseReceiptSpecifications
{
    public class GetAllPurchaseReceiptsSpec
        : BaseSpecifications<PurchaseReceipt>
    {
        public GetAllPurchaseReceiptsSpec(Guid PurchaseOrderId, Guid receiptId)
            : base(i => i.PurchaseOrderId == PurchaseOrderId && i.Id != receiptId)
        {
        }
    }
}
