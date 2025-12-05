using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.CheckIn;

namespace Khazen.Application.Validations.HRModule.AttendanceValidations
{
    public class CheckInAttendanceCommandValidator : AbstractValidator<CheckInAttendanceCommand>
    {
        public CheckInAttendanceCommandValidator()
        {
            RuleFor(x => x.Dto.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required.");

            RuleFor(x => x.Dto.Date)
                .NotEmpty().WithMessage("Date is required.")
                .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Date cannot be in the future.");

            RuleFor(x => x.Dto.CheckinTime)
                .NotEmpty().WithMessage("Check-in time is required.");

            RuleFor(x => x.Dto.Notes)
                .MaximumLength(500)
                .WithMessage("Note cannot exceed 500 characters.");
        }
    }
}
