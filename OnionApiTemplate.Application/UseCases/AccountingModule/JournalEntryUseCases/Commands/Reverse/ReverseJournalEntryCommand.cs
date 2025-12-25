namespace Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Commands.Delete
{
    public record ReverseJournalEntryCommand(Guid Id, byte[] RowVersion, string ReversedBy) : IRequest<bool>;
}
