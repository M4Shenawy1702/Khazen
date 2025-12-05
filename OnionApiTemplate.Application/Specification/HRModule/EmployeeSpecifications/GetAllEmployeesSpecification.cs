using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications
{
    public class GetAllEmployeesSpecification
        : BaseSpecifications<Employee>
    {
        public GetAllEmployeesSpecification(EmployeeQueryParameters queryParameters)
            : base(e =>
                (
                    string.IsNullOrWhiteSpace(queryParameters.Name) ||
                    e.User!.FullName.ToLower().Trim().Contains(queryParameters.Name.ToLower().Trim()) ||
                    e.User.UserName!.ToLower().Trim().Contains(queryParameters.Name.ToLower().Trim())
                )
                &&
                e.Department!.IsDeleted == true
            )
        {
            AddInclude(e => e.User!);
            AddInclude(e => e.Department!);

            switch (queryParameters.SortOption)
            {
                case EmployeeSortOptions.EmployeeNameAscending:
                    AddOrderBy(e => e.User!.FullName);
                    break;
                case EmployeeSortOptions.EmployeeNameDescending:
                    AddOrderByDesc(e => e.User!.FullName);
                    break;
            }

            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
    }
}
