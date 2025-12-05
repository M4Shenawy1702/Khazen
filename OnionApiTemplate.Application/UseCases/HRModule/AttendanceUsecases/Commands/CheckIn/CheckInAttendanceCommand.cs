using Khazen.Application.DOTs.HRModule.AttendaceDtos;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.CheckIn
{
    public record CheckInAttendanceCommand(CheckInAttendanceDto Dto) : IRequest<AttendanceDto>;
}
