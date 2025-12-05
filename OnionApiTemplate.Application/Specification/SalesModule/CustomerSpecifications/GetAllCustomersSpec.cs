using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.CustomerSpecifications
{
    public class GetAllCustomersSpec
        : BaseSpecifications<Customer>
    {
        public GetAllCustomersSpec(CustomerQueryParameters queryParameters)
            : base(c =>
                (!queryParameters.CustomerType.HasValue || c.CustomerType == queryParameters.CustomerType) &&
                (!queryParameters.IsDeleted.HasValue || c.IsDeleted == queryParameters.IsDeleted)
            )
        {
            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);

            AddOrderBy(c => c.CreatedAt);
        }
    }
}
