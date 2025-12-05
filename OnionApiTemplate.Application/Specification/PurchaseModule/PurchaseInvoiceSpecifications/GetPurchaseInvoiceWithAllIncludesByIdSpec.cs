using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications
{
    public class GetPurchaseInvoiceWithAllIncludesByIdSpec
        : BaseSpecifications<PurchaseInvoice>
    {
        public GetPurchaseInvoiceWithAllIncludesByIdSpec(Guid id)
            : base(pi => pi.Id == id)
        {
            AddInclude(pi => pi.Items);
            AddInclude(pi => pi.Supplier!);
            AddInclude(pi => pi.JournalEntry!);
            AddInclude(pi => pi.Payments!);
        }
    }
}
