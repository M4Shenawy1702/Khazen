using static Khazen.Domain.Entities.HRModule.AttendanceRecord;

namespace Khazen.Application.DOTs.HRModule.AttendaceDtos
{
    public class AttendanceDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid EmployeeId { get; set; }

        public DateOnly Date { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }

        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
        public string Notes { get; set; } = string.Empty;
    }
}
