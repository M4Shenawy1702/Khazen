using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications
{
    internal class GetAllPurchaseInvoicesCountSpec
        : BaseSpecifications<PurchaseInvoice>
    {
        public GetAllPurchaseInvoicesCountSpec(PurchaseInvoiceQueryParameters queryParameters)
          : base(pi => (!queryParameters.PaymentStatus.HasValue || pi.PaymentStatus == queryParameters.PaymentStatus))
        {
        }
    }
}
