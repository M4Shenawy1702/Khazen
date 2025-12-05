using Khazen.Application.DOTs.HRModule.AttendaceDtos;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Absent
{
    public record AbsentCommand(AbsentDto Dto) : IRequest<AttendanceDto>;
}
