using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications
{
    public class GetAllPurchaseInvoicesSpec
        : BaseSpecifications<PurchaseInvoice>
    {
        public GetAllPurchaseInvoicesSpec(PurchaseInvoiceQueryParameters queryParameters)
            : base(pi => (!queryParameters.PaymentStatus.HasValue || pi.PaymentStatus == queryParameters.PaymentStatus))
        {
            AddInclude(pi => pi.Items);
            AddInclude(pi => pi.Supplier!);
            AddInclude(pi => pi.Payments!);
            AddInclude(pi => pi.JournalEntry!);

            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
    }
}
