using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.SalesOrdersSpecifications
{
    public class GetSalesOrderByIdSpecification
        : BaseSpecifications<SalesOrder>
    {
        public GetSalesOrderByIdSpecification(Guid Id)
            : base(sp => sp.Id == Id)
        {
            AddInclude(sp => sp.Customer!);
            AddInclude(sp => sp.Items);
        }
    }
}
