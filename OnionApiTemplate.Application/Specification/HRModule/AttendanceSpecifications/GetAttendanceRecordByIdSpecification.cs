namespace Khazen.Application.BaseSpecifications.HRModule.AttendanceSpecifications
{
    internal class GetAttendanceRecordByIdSpecification
        : BaseSpecifications<AttendanceRecord>
    {
        public GetAttendanceRecordByIdSpecification(Guid AttendanceRecordId)
            : base(a => a.Id == AttendanceRecordId)
        {
        }
    }
}
