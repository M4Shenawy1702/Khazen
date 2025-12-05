using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.SalesOrdersSpecifications
{
    public class GetSalesOrderWithIncludesSpecifications
        : BaseSpecifications<SalesOrder>
    {
        public GetSalesOrderWithIncludesSpecifications(Guid Id)
            : base(so => so.Id == Id)
        {
            AddInclude(so => so.Customer!);
            AddInclude(so => so.Items!);
            AddInclude(so => so.Invoice!);
        }
    }
}
