namespace Khazen.Application.Common.QueryParameters
{
    public class SalariesQueryParameters : QueryParametersBaseClass
    {
        public Guid? EmployeeId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public SalarySortOption SalarySortOption { get; set; }
    }
    public enum SalarySortOption
    {
        PeriodAscending,
        PeriodDescending
    }
}
