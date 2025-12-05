using Khazen.Domain.Common.Enums;

namespace Khazen.Application.Common.QueryParameters
{
    public class PurchaseInvoiceQueryParameters
        : QueryParametersBaseClass
    {
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
