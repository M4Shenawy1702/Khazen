using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.CustomerSpecifications
{
    public class GetCustomerByIdWithIncludeSpec
        : BaseSpecifications<Customer>
    {
        public GetCustomerByIdWithIncludeSpec(Guid id)
            : base(c => c.Id == id && c.IsDeleted == true)
        {
            AddInclude(c => c.Invoices);
            AddInclude(c => c.Orders);
            AddInclude(c => c.User);
        }
    }
}
