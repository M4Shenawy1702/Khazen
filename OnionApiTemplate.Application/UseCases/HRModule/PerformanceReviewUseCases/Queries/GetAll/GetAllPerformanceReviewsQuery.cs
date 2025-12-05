using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Queries.GetAll
{
    public record GetAllPerformanceReviewsQuery(PerformanceReviewsQueryParameters QueryParameters) : IRequest<PaginatedResult<PerformanceReviewDto>>;
}
