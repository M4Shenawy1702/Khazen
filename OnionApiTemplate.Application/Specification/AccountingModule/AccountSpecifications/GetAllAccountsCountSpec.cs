using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.Specification.AccountingModule.AccountSpecifications
{
    public class GetAllAccountsCountSpec
        : BaseSpecifications<Account>
    {
        public GetAllAccountsCountSpec()
            : base(a => true)
        {
        }
    }
}
