using Khazen.Application.DOTs.HRModule.Department;
using Khazen.Application.DOTs.HRModule.DepartmentDtos;

namespace Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Create
{
    public record CreateDepartmentCommand(CreateDepartmentDto Dto, string CurrentUserId) : IRequest<DepartmentDetailsDto>;
}
