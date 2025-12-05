using Khazen.Domain.Common.Enums;

namespace Khazen.Application.Common.QueryParameters
{
    public class PurchaseOrdersQueryParameters : QueryParametersBaseClass
    {
        public PurchaseOrderStatus? Status { get; set; }
    }
}
