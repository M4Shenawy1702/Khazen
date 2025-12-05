namespace Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Delete
{
    public record ToggleAccountByIdCommand(Guid Id, string ToggledBy) : IRequest<bool>;
}
