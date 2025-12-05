namespace Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Queries.GetById
{
    public record GetPerformanceReviewByIdQuery(Guid Id) : IRequest<PerformanceReviewDto>;
}
