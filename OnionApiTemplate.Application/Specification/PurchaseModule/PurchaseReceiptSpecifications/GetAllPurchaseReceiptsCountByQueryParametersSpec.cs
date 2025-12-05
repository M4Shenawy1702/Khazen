using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchaseReceiptSpecifications
{
    public class GetAllPurchaseReceiptsCountByQueryParametersSpec
        : BaseSpecifications<PurchaseReceipt>
    {
        public GetAllPurchaseReceiptsCountByQueryParametersSpec(PurchaseReceiptsQueryParameters QueryParameters)
            : base(r => r.ReceiptDate >= QueryParameters.StatDate && r.ReceiptDate <= QueryParameters.EndDate)
        {
        }
    }
}
