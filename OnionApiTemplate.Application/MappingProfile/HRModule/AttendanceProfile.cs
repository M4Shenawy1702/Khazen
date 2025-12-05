using Khazen.Application.DOTs.HRModule.AttendaceDtos;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Absent;
using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.CheckIn;

namespace Khazen.Application.MappingProfile
{
    public class AttendanceProfile : Profile
    {
        public AttendanceProfile()
        {
            CreateMap<AttendanceRecord, AttendanceDto>();

            CreateMap<CheckInAttendanceCommand, AttendanceRecord>();
            CreateMap<AbsentCommand, AttendanceRecord>();

        }
    }
}
