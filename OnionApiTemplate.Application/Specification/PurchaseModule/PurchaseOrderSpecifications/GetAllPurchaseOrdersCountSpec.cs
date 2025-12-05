using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchaseOrderSpecifications
{
    public class GetAllPurchaseOrdersCountSpec : BaseSpecifications<PurchaseOrder>
    {
        public GetAllPurchaseOrdersCountSpec(PurchaseOrdersQueryParameters queryParameters)
           : base(po => true)
        {

        }
    }
}
