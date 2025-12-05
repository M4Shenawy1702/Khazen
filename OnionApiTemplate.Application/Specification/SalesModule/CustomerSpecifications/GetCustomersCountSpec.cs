using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.CustomerSpecifications
{
    internal class GetCustomersCountSpec
        : BaseSpecifications<Customer>
    {
        public GetCustomersCountSpec(CustomerQueryParameters queryParameters)
    : base(c =>
        (queryParameters.CustomerType == 0 || c.CustomerType == queryParameters.CustomerType) &&
        (queryParameters.IsDeleted == false || c.IsDeleted == queryParameters.IsDeleted)
    )
        {

        }
    }
}
