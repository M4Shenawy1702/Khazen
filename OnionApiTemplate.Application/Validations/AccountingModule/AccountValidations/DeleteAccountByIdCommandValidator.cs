using Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Delete;

namespace Khazen.Application.Validations.AccountingModule.AccountValidations
{
    public class DeleteAccountByIdCommandValidator : AbstractValidator<ToggleAccountByIdCommand>
    {
        public DeleteAccountByIdCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Account Id is required.");
        }
    }
}
