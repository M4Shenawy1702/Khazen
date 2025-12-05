using Khazen.Application.DOTs.HRModule.Department;
using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Create;

namespace Khazen.Application.MappingProfile
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<CreateDepartmentCommand, Department>();
            CreateMap<Department, DepartmentDto>();
            CreateMap<Department, DepartmentDetailsDto>();
        }
    }
}
