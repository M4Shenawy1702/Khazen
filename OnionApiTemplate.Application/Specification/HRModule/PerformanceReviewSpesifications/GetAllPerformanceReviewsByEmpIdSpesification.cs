namespace Khazen.Application.BaseSpecifications.HRModule.PerformanceReviewSpesifications
{
    internal class GetAllPerformanceReviewsByEmpIdSpesification
        : BaseSpecifications<PerformanceReview>
    {
        public GetAllPerformanceReviewsByEmpIdSpesification(Guid employeeId)
            : base(p => p.EmployeeId == employeeId)
        {
            AddInclude(p => p.Employee!);
            AddInclude(p => p.Reviewer!);
        }
    }
}
