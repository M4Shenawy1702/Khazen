using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications
{
    public class GetPurchaseInvoiceByInvoiceNumberSpec
        : BaseSpecifications<PurchaseInvoice>
    {
        public GetPurchaseInvoiceByInvoiceNumberSpec(string InvoiceNumber)
            : base(pi => pi.InvoiceNumber == InvoiceNumber)
        {
        }
    }
}
