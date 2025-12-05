using Khazen.Application.UseCases.HRModule.DeductionUseCases.Commands.Add;

namespace Khazen.Application.Validations.HRModule.DeductionValidations
{
    public class AddDeductionCommandValidator : AbstractValidator<AddDeductionCommand>
    {
        public AddDeductionCommandValidator()
        {
            RuleFor(x => x.Dto.EmployeeId)
            .NotEmpty().WithMessage("Employee ID is required.");

            RuleFor(x => x.Dto.Amount)
                .GreaterThan(0).WithMessage("Deduction amount must be greater than zero.")
                .LessThanOrEqualTo(1000000).WithMessage("Deduction amount is too large.");

            RuleFor(x => x.Dto.Date)
                .NotEmpty().WithMessage("Date is required.");

            RuleFor(x => x.Dto.Reason)
                .NotEmpty().WithMessage("Reason is required.")
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
        }
    }
}
