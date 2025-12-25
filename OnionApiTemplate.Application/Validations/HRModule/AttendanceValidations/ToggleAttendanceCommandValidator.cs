using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Delete;

namespace Khazen.Application.Validations.HRModule.AttendanceValidations
{
    public class ToggleAttendanceCommandValidator : AbstractValidator<ToggleAttendanceCommand>
    {
        public ToggleAttendanceCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Attendance record ID is required to toggle the delete status.");

            RuleFor(x => x.CurrentUserId)
                .NotEmpty().WithMessage("The user ID of the administrator toggling the record is required for auditing.");
        }
    }
}