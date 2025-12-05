using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.OrderPaymentSpecification
{
    public class GetAllSalesOrderPaymentsSpecification
        : BaseSpecifications<SalesInvoicePayment>
    {
        public GetAllSalesOrderPaymentsSpecification(SalesOrderPaymentQueryParameters queryParameters)
            : base(sp => (!queryParameters.Method.HasValue || sp.Method == queryParameters.Method))
        {
            AddInclude(sp => sp.SalesInvoice!);
            AddInclude(sp => sp.JournalEntry!);
            //AddInclude(sp => sp.SafeTransactions!);
            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
    }
}
