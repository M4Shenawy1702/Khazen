using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Create;

namespace Khazen.Application.MappingProfile
{
    public class PerformanceReviewProfile : Profile
    {
        public PerformanceReviewProfile()
        {
            CreateMap<PerformanceReview, PerformanceReviewDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee!.FirstName} {src.Employee!.LastName}"))
                .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => $"{src.Reviewer!.FirstName} {src.Reviewer!.LastName}"));
            CreateMap<CreatePerformanceReviewCommand, PerformanceReview>();
        }
    }
}
