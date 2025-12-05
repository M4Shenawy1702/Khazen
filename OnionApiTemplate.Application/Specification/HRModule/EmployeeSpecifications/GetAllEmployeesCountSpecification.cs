using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications
{
    public class GetAllEmployeesCountSpecification
        : BaseSpecifications<Employee>
    {
        public GetAllEmployeesCountSpecification(EmployeeQueryParameters queryParameters)
            : base(e => string.IsNullOrWhiteSpace(queryParameters.Name) ||
                    e.User!.FullName.ToLower().Trim().Contains(queryParameters.Name.ToLower().Trim()) ||
                    e.User.UserName!.ToLower().Trim().Contains(queryParameters.Name.ToLower().Trim()))
        {

        }
    }
}
