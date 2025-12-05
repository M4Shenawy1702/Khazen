using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchasePaymentSpecificatiocs
{
    public class GetAllPurchasePaymentsCountSpecifications
        : BaseSpecifications<PurchasePayment>
    {
        public GetAllPurchasePaymentsCountSpecifications(PurchasePaymentQueryParameters QueryParameters)
            : base(pp => (!QueryParameters.Method.HasValue || pp.Method == QueryParameters.Method))
        {
        }
    }
}
