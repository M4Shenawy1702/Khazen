using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchasePaymentSpecificatiocs
{
    public class GetAllPurchasePaymentsSpecifications
        : BaseSpecifications<PurchasePayment>
    {
        public GetAllPurchasePaymentsSpecifications(PurchasePaymentQueryParameters QueryParameters)
            : base(pp => (!QueryParameters.Method.HasValue || pp.Method == QueryParameters.Method))
        {
            ApplyPagination(QueryParameters.PageSize, QueryParameters.PageIndex);
        }
    }
}
