using Khazen.Application.UseCases.HRModule.BonusUseCases.Commands.Add;

namespace Khazen.Application.Validations.HRModule.BonusValidatoions
{
    public class AddBonusCommandValidator : AbstractValidator<AddBonusCommand>
    {
        public AddBonusCommandValidator()
        {
            RuleFor(x => x.Dto.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required.");

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required.");

            RuleFor(x => x.Dto.BonusAmount)
                .GreaterThan(0).WithMessage("Bonus amount must be greater than zero.")
                .LessThanOrEqualTo(1000000).WithMessage("Bonus amount is too large.");

            RuleFor(x => x.Dto.Date)
                .NotEmpty().WithMessage("Date is required.");

            RuleFor(x => x.Dto.Reason)
                .MaximumLength(500)
                .WithMessage("Reason cannot exceed 500 characters.");
        }
    }
}
