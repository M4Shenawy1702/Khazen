using Khazen.Application.DOTs.AccountingModule.AccountDtos;

namespace Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Create
{
    public record CreateAccountCommand(CreateAccountDto Dto, string CreatedBy) : IRequest<AccountDetailsDto>;
}
