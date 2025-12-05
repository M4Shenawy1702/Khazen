using Khazen.Application.DOTs.AccountingModule.AccountDtos;

namespace Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Update
{
    public record UpdateAccountByIdCommand(Guid Id, UpdateAccountDto Dto, string ModifiedBy)
        : IRequest<AccountDetailsDto>;
}
