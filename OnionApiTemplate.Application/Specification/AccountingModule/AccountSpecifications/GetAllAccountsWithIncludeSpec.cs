using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.Specification.AccountingModule.AccountSpecifications
{
    public class GetAllAccountsSpec
        : BaseSpecifications<Account>
    {
        public GetAllAccountsSpec(AccountQueryParameters queryParameters)
            : base(a => (!queryParameters.IsDeleted.HasValue || queryParameters.IsDeleted == a.IsDeleted))
        {

            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
    }
}
