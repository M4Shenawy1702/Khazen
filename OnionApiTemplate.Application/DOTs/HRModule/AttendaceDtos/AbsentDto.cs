namespace Khazen.Application.DOTs.HRModule.AttendaceDtos
{
    public class AbsentDto
    {
        public Guid EmployeeId { get; set; }
        public DateOnly Date { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
