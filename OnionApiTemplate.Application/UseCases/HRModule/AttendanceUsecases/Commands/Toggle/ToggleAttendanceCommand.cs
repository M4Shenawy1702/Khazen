namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Delete
{
    public record ToggleAttendanceCommand(Guid Id, string CurrentUserId) : IRequest<bool>;
}
