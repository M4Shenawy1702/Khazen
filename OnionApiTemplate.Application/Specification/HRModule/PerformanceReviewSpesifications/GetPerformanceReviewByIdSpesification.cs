namespace Khazen.Application.BaseSpecifications.HRModule.PerformanceReviewSpesifications
{
    internal class GetPerformanceReviewByIdSpesification
        : BaseSpecifications<PerformanceReview>
    {
        public GetPerformanceReviewByIdSpesification(Guid Id)
            : base(p => p.Id == Id)
        {
            AddInclude(p => p.Employee!);
        }
    }
}
