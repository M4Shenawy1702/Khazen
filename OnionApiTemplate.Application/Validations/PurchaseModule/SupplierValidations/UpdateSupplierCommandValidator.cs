using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Update;

namespace Khazen.Application.Validations.PurchaseModule.SupplierValidations
{
    public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
    {
        public UpdateSupplierCommandValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Supplier name is required.")
                .MaximumLength(100).WithMessage("Supplier name cannot exceed 100 characters.");
            RuleFor(x => x.Dto.PhoneNumber)
                .NotEmpty().WithMessage("Supplier phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");
            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Supplier email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Dto.Address)
                .NotEmpty().WithMessage("Supplier address is required.")
                .MaximumLength(250).WithMessage("Supplier address cannot exceed 250 characters.");
        }
    }
}
