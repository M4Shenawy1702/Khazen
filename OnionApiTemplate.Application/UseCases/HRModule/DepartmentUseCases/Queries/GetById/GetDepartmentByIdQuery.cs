using Khazen.Application.DOTs.HRModule.Department;

namespace Khazen.Application.UseCases.HRModule.DepartmentUseCases.Queries.GetById
{
    public record GetDepartmentByIdQuery(Guid Id) : IRequest<DepartmentDetailsDto>;
}
