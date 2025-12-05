using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.SalesOrdersSpecifications
{
    public class GetAllSalesOdersCountSpecifications
        : BaseSpecifications<SalesOrder>
    {
        public GetAllSalesOdersCountSpecifications(SalesOrdersQueryParameters queryParameters)
            : base(so => true)
        {

        }
    }
}
