using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications
{
    public class GetPurchaseInvoiceByPurchaseReceiptIdSpec
        : BaseSpecifications<PurchaseInvoice>
    {
        public GetPurchaseInvoiceByPurchaseReceiptIdSpec(Guid PurchaseReceiptId)
            : base(pi => pi.PurchaseReceiptId == PurchaseReceiptId)
        {
        }
    }
}
