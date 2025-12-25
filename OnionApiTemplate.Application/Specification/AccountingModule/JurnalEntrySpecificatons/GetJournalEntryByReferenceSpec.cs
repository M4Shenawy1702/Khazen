using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons
{
    internal class GetJournalEntryByReferenceSpec : BaseSpecifications<JournalEntry>
    {
        public GetJournalEntryByReferenceSpec(Guid ReferenceId) : base(je => je.RelatedEntityId == ReferenceId)
        {
        }
    }
}
