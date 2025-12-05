namespace Khazen.Application.BaseSpecifications.HRModule.AttendanceSpecifications
{
    internal class GetAttendanceRecordByEmployeeIdAndDateSpecification
        : BaseSpecifications<AttendanceRecord>
    {
        public GetAttendanceRecordByEmployeeIdAndDateSpecification(Guid EmployeeId, DateTime Date)
            : base(x => x.EmployeeId == EmployeeId && x.Date.Month == DateTime.UtcNow.Date.Month && x.Date.Year == DateTime.UtcNow.Date.Year)
        {
        }
    }
}
