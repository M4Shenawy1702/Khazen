namespace Khazen.Application.UseCases.HRModule.AdvanceUseCases.Commands.Toggle
{
    public record ToggleAdvanceCommand(int Id, byte[] RowVersion, string CurrentUserId) : IRequest<bool>;
}
