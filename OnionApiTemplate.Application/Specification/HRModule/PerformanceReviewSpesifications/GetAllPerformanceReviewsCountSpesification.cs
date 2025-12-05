using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.BaseSpecifications.HRModule.PerformanceReviewSpesifications
{
    internal class GetAllPerformanceReviewsCountSpesification : BaseSpecifications<PerformanceReview>
    {
        public GetAllPerformanceReviewsCountSpesification(PerformanceReviewsQueryParameters spesification)
    : base(p =>
        p.ReviewDate >= spesification.From &&
        p.ReviewDate <= spesification.To)
        {
        }
    }
}
