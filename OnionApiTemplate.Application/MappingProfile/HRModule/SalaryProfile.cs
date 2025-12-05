namespace Khazen.Application.MappingProfile
{
    public class SalaryMappingProfile : Profile
    {
        public SalaryMappingProfile()
        {
            CreateMap<Salary, SalaryDto>();

        }
    }
}
