using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.BaseSpecifications.HRModule.DeductionSpecifications
{
    internal class GetAllDeductionsSpecification
        : BaseSpecifications<Deduction>
    {
        public GetAllDeductionsSpecification(DeductionQueryParameters queryParameters)
            : base(a =>
                (!queryParameters.EmployeeId.HasValue || a.EmployeeId == queryParameters.EmployeeId) &&
                (!queryParameters.From.HasValue || a.Date >= queryParameters.From.Value) &&
                (!queryParameters.To.HasValue || a.Date <= queryParameters.To.Value)
            )
        {
            AddInclude(d => d.Employee);
            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
    }
}
