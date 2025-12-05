using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.SalesInvoicesSpecifications
{
    public class GetSalesInvoiceWithIncludesSpecifications
        : BaseSpecifications<SalesInvoice>
    {
        public GetSalesInvoiceWithIncludesSpecifications(Guid Id) :
            base(si => si.Id == Id)
        {
            AddInclude(si => si.Customer!);
            AddInclude(si => si.SalesOrder!);
            AddInclude(si => si.Payments!);
        }
    }
}
