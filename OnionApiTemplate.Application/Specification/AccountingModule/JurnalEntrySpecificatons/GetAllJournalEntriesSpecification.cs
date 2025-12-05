using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons
{
    public class GetAllJournalEntriesSpecification
        : BaseSpecifications<JournalEntry>
    {
        public GetAllJournalEntriesSpecification(JurnalEntryQueryParameters queryParameters)
           : base(je =>
                !queryParameters.TransactionType.HasValue ||
                je.TransactionType == queryParameters.TransactionType.Value
            )
        {
            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
        public GetAllJournalEntriesSpecification(List<Guid> ids)
           : base(je => ids.Contains(je.Id))
        {
            AddInclude(je => je.Lines);
        }
    }
}
