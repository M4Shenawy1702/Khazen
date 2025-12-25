using Khazen.Application.DOTs.HRModule.AttendaceDtos;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.CheckIn
{
    public record CheckInAttendanceCommand(CheckInDto Dto, string CurrentUserId) : IRequest<AttendanceDto>;
}
