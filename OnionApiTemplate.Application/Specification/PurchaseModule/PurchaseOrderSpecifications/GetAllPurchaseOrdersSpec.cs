using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchaseOrderSpecifications
{
    public class GetAllPurchaseOrdersSpec
        : BaseSpecifications<PurchaseOrder>
    {
        public GetAllPurchaseOrdersSpec(PurchaseOrdersQueryParameters queryParameters)
            : base(po =>
                (!queryParameters.Status.HasValue || po.Status == queryParameters.Status))
        {
            AddInclude(po => po.Items);
            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
    }
}
