using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.Specification.AccountingModule.AccountSpecifications
{
    internal class GetAccountByNameSpecification : BaseSpecifications<Account>
    {
        public GetAccountByNameSpecification(string Name)
            : base(a => a.Name == Name)
        {
        }
    }
}
