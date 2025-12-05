using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.AccountingModule.JournalEntryDots;

namespace Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Queries.GetAll
{
    public record GetPaginatedJournalEntriesQuery(JurnalEntryQueryParameters QueryParameters) : IRequest<PaginatedResult<JournalEntryDto>>;
}
