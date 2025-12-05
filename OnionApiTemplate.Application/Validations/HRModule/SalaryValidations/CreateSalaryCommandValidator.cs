using Khazen.Application.UseCases.HRModule.SalaryUseCases.Commands.Create;

namespace Khazen.Application.Validations.HRModule.SalaryValidations
{
    public class CreateSalaryCommandValidator : AbstractValidator<CreateSalaryCommand>
    {
        public CreateSalaryCommandValidator()
        {
            RuleFor(x => x.Dto.EmployeeId)
                .NotEmpty()
                .WithMessage("Employee ID is required.");

            RuleFor(x => x.Dto.SalaryDate)
                .NotEmpty()
                .WithMessage("Salary date is required.")
                .Must(date => date <= DateTime.Today)
                .WithMessage("Salary date cannot be in the future.");

            RuleFor(x => x.Dto.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
