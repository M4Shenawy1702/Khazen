using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Create;

namespace Khazen.Application.MappingProfile
{
    public class PerformanceReviewProfile : Profile
    {
        public PerformanceReviewProfile()
        {
            CreateMap<PerformanceReview, PerformanceReviewDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee!.FirstName} {src.Employee!.LastName}"));
            CreateMap<CreatePerformanceReviewCommand, PerformanceReview>();
        }
    }
}
