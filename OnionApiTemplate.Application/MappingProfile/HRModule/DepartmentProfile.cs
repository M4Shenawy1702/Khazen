using Khazen.Application.DOTs.HRModule.Department;
using Khazen.Application.DOTs.HRModule.DepartmentDtos;

namespace Khazen.Application.MappingProfile
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();
            CreateMap<Department, DepartmentDto>();
            CreateMap<Department, DepartmentDetailsDto>();
        }
    }
}
