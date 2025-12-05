using Khazen.Application.DOTs.HRModule.AttendaceDtos;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Update
{
    public record UpdateAttendanceCommand(Guid Id, UpdateAttendanceDto Dto)
        : IRequest<AttendanceDto>;
}
