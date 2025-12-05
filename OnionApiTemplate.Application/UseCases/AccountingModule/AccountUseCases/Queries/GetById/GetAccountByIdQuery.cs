using Khazen.Application.DOTs.AccountingModule.AccountDtos;

namespace Khazen.Application.UseCases.AccountingModule.AccountUseCases.Queries.GetById
{
    public record GetAccountByIdQuery(Guid Id) : IRequest<AccountDetailsDto>;
}
