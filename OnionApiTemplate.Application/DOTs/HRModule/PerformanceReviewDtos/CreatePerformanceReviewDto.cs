namespace Khazen.Application.DOTs.HRModule.PerformanceReviewDtos
{
    public class CreatePerformanceReviewDto
    {
        public Guid EmployeeId { get; set; }
        public Guid ReviewerId { get; set; }
        public DateOnly ReviewDate { get; set; }
        public string? Comments { get; set; }
        public int Rate { get; set; }
        public string? ActionPlan { get; set; }
    }
}
