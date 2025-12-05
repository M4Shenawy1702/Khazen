using Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Update;

namespace Khazen.Application.Validations.AccountingModule.AccountValidations
{
    public class UpdateAccountByIdCommandValidator : AbstractValidator<UpdateAccountByIdCommand>
    {
        public UpdateAccountByIdCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Account Id is required.");

            RuleFor(x => x.Dto.Code)
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.");

            RuleFor(x => x.Dto.Name)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Dto.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters.");
        }
    }
}
