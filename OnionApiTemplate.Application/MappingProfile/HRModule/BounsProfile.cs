using Khazen.Application.DOTs.HRModule.BonusDtos;
using Khazen.Application.UseCases.HRModule.BonusUseCases.Commands.Add;

namespace Khazen.Application.MappingProfile
{
    public class BounsProfile : Profile
    {
        public BounsProfile()
        {
            CreateMap<AddBonusCommand, Bonus>();
            CreateMap<Bonus, BonusDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee!.FirstName} {src.Employee!.LastName}"));
            CreateMap<Bonus, BonusDetailsDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee!.FirstName} {src.Employee!.LastName}"));
        }
    }
}
