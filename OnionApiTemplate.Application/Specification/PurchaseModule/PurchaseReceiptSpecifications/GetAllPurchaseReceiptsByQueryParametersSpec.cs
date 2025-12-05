using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchaseReceiptSpecifications
{
    public class GetAllPurchaseReceiptsByQueryParametersSpec
        : BaseSpecifications<PurchaseReceipt>
    {
        public GetAllPurchaseReceiptsByQueryParametersSpec(PurchaseReceiptsQueryParameters queryParameters)
            : base(r =>
                (!queryParameters.StatDate.HasValue || r.ReceiptDate >= queryParameters.StatDate.Value) &&
                (!queryParameters.EndDate.HasValue || r.ReceiptDate <= queryParameters.EndDate.Value)
            )
        {
            AddInclude(r => r.Items);
            AddInclude("Items.Product");

            AddInclude(r => r.PurchaseOrder!);
            AddInclude("PurchaseOrder.Supplier");

            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
    }
}
