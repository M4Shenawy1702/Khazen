namespace Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Delete
{
    public record ToggleAccountByIdCommand(Guid Id, byte[] RowVersion, string CurrentUserId) : IRequest<bool>;
}
