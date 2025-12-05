using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons
{
    public class GetAllJournalEntriesCountSpecification
        : BaseSpecifications<JournalEntry>
    {
        public GetAllJournalEntriesCountSpecification(JurnalEntryQueryParameters queryParameters)
            : base(je => true)
        {

        }
    }
}
