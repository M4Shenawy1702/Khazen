namespace Khazen.Application.Common.QueryParameters
{
    public class PerformanceReviewsQueryParameters : QueryParametersBaseClass
    {
        public Guid? EmployeeId { get; set; }
        public DateOnly? From { get; set; }
        public DateOnly? To { get; set; }
    }
}
