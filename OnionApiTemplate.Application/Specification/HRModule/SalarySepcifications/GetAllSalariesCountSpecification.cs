using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.Specification.HRModule.SalarySepcifications
{
    internal class GetAllSalariesCountSpecification
        : BaseSpecifications<Salary>
    {
        public GetAllSalariesCountSpecification(SalariesQueryParameters queryParameters)
            : base(s =>
                (!queryParameters.EmployeeId.HasValue || s.EmployeeId == queryParameters.EmployeeId.Value) &&
                (!queryParameters.From.HasValue || s.SalaryDate >= queryParameters.From.Value) &&
                (!queryParameters.To.HasValue || s.SalaryDate <= queryParameters.To.Value))
        {
            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
    }
}
