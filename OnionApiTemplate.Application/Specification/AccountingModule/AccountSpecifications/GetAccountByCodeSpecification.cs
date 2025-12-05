using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.Specification.AccountingModule.AccountSpecifications
{
    internal class GetAccountByCodeSpecification : BaseSpecifications<Account>
    {
        public GetAccountByCodeSpecification(string Code)
            : base(a => a.Code == Code)
        {
        }
    }
}
