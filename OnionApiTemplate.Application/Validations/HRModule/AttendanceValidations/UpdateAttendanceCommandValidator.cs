using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Update;

namespace Khazen.Application.Validations.HRModule.AttendanceValidations
{
    public class UpdateAttendanceCommandValidator : AbstractValidator<UpdateAttendanceCommand>
    {
        public UpdateAttendanceCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Attendance ID is required.");

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Employee ID is required.");

            RuleFor(x => x.Dto.Date)
                .NotEmpty().WithMessage("Date is required.")
                .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Date cannot be in the future.");

            RuleFor(x => x.Dto.Status)
                .IsInEnum().WithMessage("Invalid attendance status.");

            RuleFor(x => x.Dto.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
