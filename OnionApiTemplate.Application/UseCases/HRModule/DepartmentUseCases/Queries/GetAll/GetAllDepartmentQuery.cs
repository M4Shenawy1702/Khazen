using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.Department;

namespace Khazen.Application.UseCases.HRModule.DepartmentUseCases.Queries.GetAll
{
    public record GetAllDepartmentQuery(DepartmentQueryParameters QueryParameters) : IRequest<PaginatedResult<DepartmentDto>>;
}
