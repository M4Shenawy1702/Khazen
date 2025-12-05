using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.SalesOrdersSpecifications
{
    public class GetAllSalesOdersSpecifications
        : BaseSpecifications<SalesOrder>
    {
        public GetAllSalesOdersSpecifications(SalesOrdersQueryParameters queryParameters)
            : base(so => true)
        {
            AddInclude(so => so.Customer!);
            AddInclude(so => so.Invoice!);

            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
    }
}
