using Khazen.Application.UseCases.HRModule.AdvanceUseCases.Commands.Create;

namespace Khazen.Application.Validations.HRModule.AdvanceValidations
{
    public class AddAdvanceCommandValidations : AbstractValidator<AddAdvanceCommand>
    {
        public AddAdvanceCommandValidations()
        {
            RuleFor(x => x.Dto.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required");
            RuleFor(x => x.Dto.Amount)
                .GreaterThan(0).WithMessage("Advance amount must be greater than zero");
            RuleFor(x => x.Dto.Reason)
                .NotEmpty().WithMessage("Reason for advance is required")
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");
        }
    }
}
