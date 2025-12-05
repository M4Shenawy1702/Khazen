using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Update;

namespace Khazen.Application.Validations.SalesModule.CustomerValidations
{
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Customer Id is required.");

            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Customer name is required.")
                .MaximumLength(100).WithMessage("Customer name cannot exceed 100 characters.");

            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Dto.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{7,15}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Dto.Address)
                .MaximumLength(250).WithMessage("Address cannot exceed 250 characters.");
        }
    }
}
