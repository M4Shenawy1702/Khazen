using Khazen.Application.DOTs.HRModule.AttendaceDtos;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.CheckOut
{
    public record CheckOutCommand(Guid AttendanceId) : IRequest<AttendanceDto>;
}
