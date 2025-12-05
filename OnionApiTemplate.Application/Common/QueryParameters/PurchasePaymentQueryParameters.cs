using Khazen.Domain.Common.Enums;

namespace Khazen.Application.Common.QueryParameters
{
    public class PurchasePaymentQueryParameters
        : QueryParametersBaseClass
    {
        public PaymentMethod? Method { get; set; }
    }
}
