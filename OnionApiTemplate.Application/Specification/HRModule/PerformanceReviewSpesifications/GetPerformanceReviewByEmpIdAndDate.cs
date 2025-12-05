namespace Khazen.Application.BaseSpecifications.HRModule.PerformanceReviewSpesifications
{
    internal class GetPerformanceReviewByEmpIdAndDate
        : BaseSpecifications<PerformanceReview>
    {
        public GetPerformanceReviewByEmpIdAndDate(Guid EmployeeId)
            : base(p => p.EmployeeId == EmployeeId && p.CreatedAt.Month == DateTime.Now.Month && p.CreatedAt.Year == DateTime.Now.Year)
        {
        }
    }
}
