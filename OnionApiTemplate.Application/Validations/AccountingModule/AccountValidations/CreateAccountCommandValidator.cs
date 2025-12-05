using Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Create;

namespace Khazen.Application.Validations.AccountingModule.AccountValidations
{
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {
            RuleFor(x => x.Dto.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");

            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Dto.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(x => x.Dto.AccountType)
                .IsInEnum().WithMessage("Invalid account type.");
        }
    }
}
