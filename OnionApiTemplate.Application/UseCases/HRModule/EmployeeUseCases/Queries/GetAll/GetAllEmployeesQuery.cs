using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.Employee;
using MediatR;

namespace Khazen.Application.UseCases.HRModule.EmployeeUsecases.Queries.GetAll
{
    public record GetAllEmployeesQuery(EmployeeQueryParameters queryParameters) : IRequest<PaginatedResult<EmployeeDto>>;
}
