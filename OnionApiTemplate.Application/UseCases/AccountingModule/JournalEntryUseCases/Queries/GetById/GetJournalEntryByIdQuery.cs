using Khazen.Application.DOTs.AccountingModule.JournalEntryDots;

namespace Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Queries.GetById
{
    public record GetJournalEntryByIdQuery(Guid Id) : IRequest<JournalEntryDetailsDto>;
}
