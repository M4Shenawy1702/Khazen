using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.UseCases.HRModule.EmployeeUsecases.Queries.GetAll
{
    public record GetAllEmployeesQuery(EmployeeQueryParameters QueryParameters) : IRequest<PaginatedResult<EmployeeDto>>;
}
