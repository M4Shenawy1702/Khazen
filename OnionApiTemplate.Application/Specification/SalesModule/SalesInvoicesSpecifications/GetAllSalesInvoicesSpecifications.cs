using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.SalesInvoicesSpecifications
{
    public class GetAllSalesInvoicesSpecifications
        : BaseSpecifications<SalesInvoice>
    {
        public GetAllSalesInvoicesSpecifications(SalesInvoicesQueryParameters queryParameters)
            : base(si => (!queryParameters.CustomerId.HasValue || si.CustomerId == queryParameters.CustomerId)
            && (!queryParameters.InvoiceStatus.HasValue || si.InvoiceStatus == queryParameters.InvoiceStatus))
        {
            AddInclude(si => si.Customer!);
            AddInclude(si => si.SalesOrder!);
            AddInclude(si => si.Payments!);
            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
    }
}
