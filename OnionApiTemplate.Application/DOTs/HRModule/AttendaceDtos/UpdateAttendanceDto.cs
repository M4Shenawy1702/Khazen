using static Khazen.Domain.Entities.HRModule.AttendanceRecord;

namespace Khazen.Application.DOTs.HRModule.AttendaceDtos
{
    public class UpdateAttendanceDto
    {
        public DateOnly Date { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public string? Notes { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
