using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.SalesInvoicesSpecifications
{
    public class GetAllSalesInvoicesCountSpecifications
        : BaseSpecifications<SalesInvoice>
    {
        public GetAllSalesInvoicesCountSpecifications(SalesInvoicesQueryParameters queryParameters)
            : base(si => (!queryParameters.CustomerId.HasValue || si.CustomerId == queryParameters.CustomerId)
            && (!queryParameters.InvoiceStatus.HasValue || si.InvoiceStatus == queryParameters.InvoiceStatus))
        {

        }
    }
}
