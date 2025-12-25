namespace Khazen.Application.DOTs.HRModule.AttendaceDtos
{
    public class CheckInDto
    {
        public Guid EmployeeId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly CheckinTime { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
