using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.BaseSpecifications.HRModule.AdvanceSpecifications
{
    internal class GetAllAdvanceSpecification : BaseSpecifications<Advance>
    {
        public GetAllAdvanceSpecification(AdvanceQueryParameters queryParameters)
            : base(a =>
                (!queryParameters.EmployeeId.HasValue || a.EmployeeId == queryParameters.EmployeeId) &&
                (!queryParameters.From.HasValue || a.Date >= queryParameters.From.Value) &&
                (!queryParameters.To.HasValue || a.Date <= queryParameters.To.Value)
            )
        {
            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
            AddOrderBy(x => x.Date);
        }
    }
}
