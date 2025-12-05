namespace Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Delete
{
    public record TogglePerformanceReviewCommand(Guid Id, string ModifiedBy) : IRequest<bool>;
}
