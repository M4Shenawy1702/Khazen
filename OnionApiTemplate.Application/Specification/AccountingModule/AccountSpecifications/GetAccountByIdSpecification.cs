using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.Specification.AccountingModule.AccountSpecifications
{
    internal class GetAccountByIdSpecification
        : BaseSpecifications<Account>
    {
        public GetAccountByIdSpecification(Guid accountId)
            : base(a => a.Id == accountId)
        {
            AddInclude(a => a.Children);
        }
    }
}
