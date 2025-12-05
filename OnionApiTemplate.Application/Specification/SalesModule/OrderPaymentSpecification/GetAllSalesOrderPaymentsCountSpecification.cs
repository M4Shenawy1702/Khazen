using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.OrderPaymentSpecification
{
    public class GetAllSalesOrderPaymentsCountSpecification
        : BaseSpecifications<SalesInvoicePayment>
    {
        public GetAllSalesOrderPaymentsCountSpecification(SalesOrderPaymentQueryParameters queryParameters)
            : base(sp => (!queryParameters.Method.HasValue || sp.Method == queryParameters.Method))
        {
        }
    }
}
