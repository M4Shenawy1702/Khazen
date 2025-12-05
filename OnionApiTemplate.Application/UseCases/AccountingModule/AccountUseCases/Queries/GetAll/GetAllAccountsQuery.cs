using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.AccountingModule.AccountDtos;

namespace Khazen.Application.UseCases.AccountingModule.AccountUseCases.Queries.GetAll
{
    public record GetAllAccountsQuery(AccountQueryParameters QueryParameters) : IRequest<PaginatedResult<AccountDto>>;
}
