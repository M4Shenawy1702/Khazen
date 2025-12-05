using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.BaseSpecifications.HRModule.DepartmentSpecifications
{
    internal class GetAllDepartmentSpecification
        : BaseSpecifications<Department>
    {
        public GetAllDepartmentSpecification(DepartmentQueryParameters queryParameters)
            : base(d =>
                (queryParameters.DepartmentId == null || d.Id == queryParameters.DepartmentId) &&
                (string.IsNullOrWhiteSpace(queryParameters.DepartmentName) ||
                 d.Name.ToLower().Trim().Contains(queryParameters.DepartmentName.ToLower().Trim())) &&
                d.IsDeleted == queryParameters.IsActive
            )
        {
            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
    }
}
