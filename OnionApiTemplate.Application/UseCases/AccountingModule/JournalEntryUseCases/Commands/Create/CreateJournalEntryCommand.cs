using Khazen.Application.DOTs.AccountingModule.JournalEntryDots;

namespace Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Commands.Create
{
    public record CreateJournalEntryCommand(CreateJournalEntryDto Dto, string CreatedBy) : IRequest<JournalEntryDetailsDto>;
}
