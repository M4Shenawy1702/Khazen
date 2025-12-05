namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Delete
{
    public record ToggleAttendanceCommand(Guid Id) : IRequest<bool>;
}
