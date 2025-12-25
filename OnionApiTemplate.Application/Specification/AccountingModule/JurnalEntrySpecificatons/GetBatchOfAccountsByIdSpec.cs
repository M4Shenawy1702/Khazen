using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons
{
    internal class GetBatchOfAccountsByIdSpec
        : BaseSpecifications<Account>
    {
        public GetBatchOfAccountsByIdSpec(HashSet<Guid> Ids)
            : base(a => Ids.Contains(a.Id))
        {
            AddInclude(a => a.Safe!);
        }
    }
}
