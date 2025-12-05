using Khazen.Application.DOTs.HRModule.Department;
using Khazen.Application.DOTs.HRModule.DepartmentDtos;

namespace Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Update
{
    public record UpdateDepartmentCommand(
        Guid Id, UpdateDepartmentDto Dto, string ModifiedBy) : IRequest<DepartmentDetailsDto>;
}
