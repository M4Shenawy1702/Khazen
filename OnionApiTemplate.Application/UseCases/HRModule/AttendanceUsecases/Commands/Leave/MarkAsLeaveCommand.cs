using Khazen.Application.DOTs.HRModule.AttendaceDtos;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Leave
{
    public record MarkAsLeaveCommand(MarkAsLeaveDto Dto, string CurrentUserId) : IRequest<AttendanceDto>;
}
