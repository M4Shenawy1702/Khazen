using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Absent;

namespace Khazen.Application.Validations.HRModule.AttendanceValidations
{
    public class AbsentCommandValidator : AbstractValidator<AbsentCommand>
    {
        public AbsentCommandValidator()
        {
            RuleFor(x => x.Dto.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required.");

            RuleFor(x => x.Dto.Date)
                .NotEmpty().WithMessage("Date is required.")
                .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Date cannot be in the future.");

            RuleFor(x => x.Dto.Notes)
                .MaximumLength(500)
                .WithMessage("Note cannot exceed 500 characters.");
        }
    }
}
