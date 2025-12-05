using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchasePaymentSpecificatiocs
{
    public class GetPurcasePaymentByIdWithIncludesSpecifications
        : BaseSpecifications<PurchasePayment>
    {
        public GetPurcasePaymentByIdWithIncludesSpecifications(Guid Id)
            : base(pp => pp.Id == Id)
        {
            AddInclude(pp => pp.JournalEntry!);
            AddInclude(pp => pp.PurchaseInvoice);
            AddInclude(pp => pp.SafeTransaction);
        }
    }
}
