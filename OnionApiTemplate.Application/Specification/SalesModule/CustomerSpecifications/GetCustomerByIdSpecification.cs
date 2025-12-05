using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.CustomerSpecifications
{
    internal class GetCustomerByIdSpecification
        : BaseSpecifications<Customer>
    {
        public GetCustomerByIdSpecification(Guid id)
            : base(c => c.Id == id && c.IsDeleted == true)
        {
        }
    }
}
