using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Create;

namespace Khazen.Application.Validations.SalesModule.CustomerValidations
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Customer name is required.")
                .MaximumLength(100).WithMessage("Customer name must not exceed 100 characters.");

            RuleFor(x => x.Dto.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9]{7,15}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Dto.Address)
                .MaximumLength(250).WithMessage("Address must not exceed 250 characters.");

            RuleFor(x => x.Dto.CustomerType)
                .IsInEnum().WithMessage("Invalid customer type.");

        }
    }
}
