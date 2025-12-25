using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.BaseSpecifications.HRModule.PerformanceReviewSpesifications
{
    internal class GetAllPerformanceReviewsSpesification : BaseSpecifications<PerformanceReview>
    {
        public GetAllPerformanceReviewsSpesification(PerformanceReviewsQueryParameters queryParameters)
            : base(r =>
                (!queryParameters.EmployeeId.HasValue ||
                    r.EmployeeId == queryParameters.EmployeeId.Value) &&

                (!queryParameters.From.HasValue ||
                    r.ReviewDate >= queryParameters.From.Value) &&

                (!queryParameters.To.HasValue ||
                    r.ReviewDate <= queryParameters.To.Value)
            )

        {
            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);

            AddInclude(p => p.Employee!);

            AddOrderBy(p => p.ReviewDate);
        }
    }
}
