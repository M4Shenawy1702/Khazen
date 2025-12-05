namespace Khazen.Domain.Entities.HRModule
{
    public class AttendanceRecord : BaseEntity<Guid>
    {
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public TimeOnly? CheckInTime { get; private set; }
        public TimeOnly? CheckOutTime { get; private set; }

        public AttendanceStatus Status { get; private set; } = AttendanceStatus.Absent;
        public string Notes { get; private set; } = string.Empty;

        public void CheckIn(TimeOnly? time)
        {
            CheckInTime = time ?? this.CheckInTime;
            Status = AttendanceStatus.Present;
        }


        public void CheckOut(TimeOnly? time)
        {
            CheckOutTime = time ?? this.CheckOutTime;
        }

        public void MarkLeave(string? note = null)
        {
            Status = AttendanceStatus.Leave;
            Notes = note ?? Notes;
        }
        public void MarkAsAbsent()
        {
            Status = AttendanceStatus.Absent;
        }

        public void UpdateStatus(AttendanceStatus attendanceStatus)
        {
            Status = attendanceStatus;
        }
        public void UpdateNotes(string? note)
        {
            Notes = note ?? Notes;
        }
        public enum AttendanceStatus
        {
            Present,
            Absent,
            Leave
        }
    }
}