using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.BaseSpecifications.HRModule.DepartmentSpecifications
{
    internal class GetAllDepartmentCountSpecification
        : BaseSpecifications<Department>
    {
        public GetAllDepartmentCountSpecification(DepartmentQueryParameters queryParameters)
                  : base(d =>
                   (queryParameters.DepartmentId == null || d.Id == queryParameters.DepartmentId) &&
                   (string.IsNullOrWhiteSpace(queryParameters.DepartmentName) || d.Name.ToLower().Trim().Contains(queryParameters.DepartmentName.ToLower().Trim()))
                   )
        {
            AddInclude(d => d.Employees);
        }
    }
}
