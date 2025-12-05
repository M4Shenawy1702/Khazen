using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.BaseSpecifications.HRModule.SalarySepcifications
{
    internal class GetAllSalariesSpecification
        : BaseSpecifications<Salary>
    {
        public GetAllSalariesSpecification(SalariesQueryParameters queryParameters)
            : base(s =>
                (!queryParameters.EmployeeId.HasValue || s.EmployeeId == queryParameters.EmployeeId.Value) &&
                (!queryParameters.From.HasValue || s.SalaryDate >= queryParameters.From.Value) &&
                (!queryParameters.To.HasValue || s.SalaryDate <= queryParameters.To.Value))
        {
            AddInclude(s => s.Employee!);

            ApplySorting(queryParameters);

            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }

        private void ApplySorting(SalariesQueryParameters queryParameters)
        {
            switch (queryParameters.SalarySortOption)
            {
                case SalarySortOption.PeriodDescending:
                    AddOrderByDesc(s => s.SalaryDate);
                    break;
                case SalarySortOption.PeriodAscending:
                    AddOrderBy(s => s.SalaryDate);
                    break;
                default:
                    AddOrderByDesc(s => s.SalaryDate);
                    break;
            }
        }
    }
}
