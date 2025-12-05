using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.Specification.AccountingModule.AccountSpecifications
{
    public class GetAccountByIdWithIncludeSpecification
        : BaseSpecifications<Account>
    {
        public GetAccountByIdWithIncludeSpecification(Guid Id)
            : base(a => a.Id == Id)
        {
            AddInclude(a => a.Parent!);
            AddInclude(a => a.Children);
        }
    }
}
