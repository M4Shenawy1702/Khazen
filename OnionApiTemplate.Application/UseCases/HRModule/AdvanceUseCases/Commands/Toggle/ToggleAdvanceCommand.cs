namespace Khazen.Application.UseCases.HRModule.AdvanceUseCases.Commands.Toggle
{
    public record ToggleAdvanceCommand(int Id, string ModifiedBy) : IRequest<bool>;
}
