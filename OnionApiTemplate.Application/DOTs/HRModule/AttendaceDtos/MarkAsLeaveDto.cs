namespace Khazen.Application.DOTs.HRModule.AttendaceDtos
{
    public class MarkAsLeaveDto
    {
        public Guid EmployeeId { get; set; }
        public DateOnly? Date { get; set; }
        public string Notes { get; private set; } = string.Empty;
    }
}
