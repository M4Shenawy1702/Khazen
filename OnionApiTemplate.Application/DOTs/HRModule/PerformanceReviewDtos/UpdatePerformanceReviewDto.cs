namespace Khazen.Application.DOTs.HRModule.PerformanceReviewDtos
{
    public class UpdatePerformanceReviewDto
    {

        public DateOnly ReviewDate { get; set; }
        public string? Comments { get; set; }
        public int Rate { get; set; }
        public string? ActionPlans { get; set; }
    }
}
