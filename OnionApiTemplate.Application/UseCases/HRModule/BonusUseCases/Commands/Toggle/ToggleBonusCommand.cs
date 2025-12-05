namespace Khazen.Application.UseCases.HRModule.BonusUseCases.Commands.Delete
{
    public record ToggleBonusCommand(int Id, string ModifiedBy) : IRequest<bool>;
}
