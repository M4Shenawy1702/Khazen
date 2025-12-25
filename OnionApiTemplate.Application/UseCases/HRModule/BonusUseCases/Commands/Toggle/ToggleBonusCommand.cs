namespace Khazen.Application.UseCases.HRModule.BonusUseCases.Commands.Delete
{
    public record ToggleBonusCommand(Guid Id, string CurrentUserId) : IRequest<bool>;
}
