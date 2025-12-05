namespace Khazen.Application.Common.QueryParameters
{
    public class BonusQueryParameters : QueryParametersBaseClass
    {
        public Guid? EmployeeId { get; set; }
        public DateOnly? From { get; set; }
        public DateOnly? To { get; set; }
    }
}
