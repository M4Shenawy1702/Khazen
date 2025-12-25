namespace Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Create
{
    public record CreatePerformanceReviewCommand(CreatePerformanceReviewDto Dto, string CurrentUserId) : IRequest<PerformanceReviewDto>;
}
