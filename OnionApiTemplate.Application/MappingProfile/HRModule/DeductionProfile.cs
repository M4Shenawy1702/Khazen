using Khazen.Application.DOTs.HRModule.Deduction;
using Khazen.Application.DOTs.HRModule.DeductionDtos;

namespace Khazen.Application.MappingProfile
{
    public class DeductionProfile : Profile
    {
        public DeductionProfile()
        {
            CreateMap<Deduction, DeductionDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee!.FirstName} {src.Employee!.LastName}"));
            CreateMap<Deduction, DeductionDetailsDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee!.FirstName} {src.Employee!.LastName}"));


        }
    }
}
