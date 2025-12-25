namespace Khazen.Application.DOTs.HRModule.AttendaceDtos
{
    public class CheckOutDto
    {
        public Guid EmployeeId { get; set; }
        public TimeOnly? CheckOutTime = null;
    }
}
