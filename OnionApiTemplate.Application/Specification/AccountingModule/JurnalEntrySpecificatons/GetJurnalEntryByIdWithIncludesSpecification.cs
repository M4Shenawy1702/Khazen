using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons
{
    public class GetJurnalEntryByIdWithIncludesSpecification
        : BaseSpecifications<JournalEntry>
    {
        public GetJurnalEntryByIdWithIncludesSpecification(Guid id)
            : base(je => je.Id == id)
        {
            AddInclude(je => je.Lines);
            AddInclude("Lines.Account");
        }
    }
}
