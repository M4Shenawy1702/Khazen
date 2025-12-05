using Khazen.Domain.Common.Enums;

namespace Khazen.Application.Common.QueryParameters
{
    public class SalesOrderPaymentQueryParameters : QueryParametersBaseClass
    {
        public PaymentMethod? Method { get; set; }
    }
}
