using Khazen.Application.DOTs.HRModule.AdvanceDtos;
using Khazen.Application.UseCases.HRModule.AdvanceUseCases.Commands.Create;

namespace Khazen.Application.MappingProfile.HRModule
{
    public class AdvanceProfile : Profile
    {
        public AdvanceProfile()
        {
            CreateMap<AddAdvanceCommand, Advance>();
            CreateMap<Advance, AdvanceDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee!.FirstName} {src.Employee!.LastName}"));
            CreateMap<Advance, AdvanceDetailsDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee!.FirstName} {src.Employee!.LastName}"));
        }
    }
}
