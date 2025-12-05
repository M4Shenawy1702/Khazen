namespace Khazen.Application.DOTs.HRModule.PerformanceReviewDtos
{
    public class PerformanceReviewDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string EmployeeName { get; set; } = string.Empty;
        public string ReviewerName { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public DateOnly ReviewDate { get; set; }
        public int Rate { get; set; }
        public string ActionPlan { get; set; } = string.Empty;
    }
}
