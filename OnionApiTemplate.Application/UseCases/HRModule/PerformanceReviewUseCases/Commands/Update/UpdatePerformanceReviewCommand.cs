namespace Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Update
{
    public record UpdatePerformanceReviewCommand(Guid Id, UpdatePerformanceReviewDto Dto, string ModifiedBy) : IRequest<PerformanceReviewDto>;
}
