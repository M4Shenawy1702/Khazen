using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Leave;

namespace Khazen.Application.Validations.HRModule.AttendanceValidations
{
    public class MarkAsLeaveCommandValidator : AbstractValidator<MarkAsLeaveCommand>
    {
        public MarkAsLeaveCommandValidator()
        {
            RuleFor(x => x.Dto)
                .NotNull().WithMessage("Mark as Leave data (DTO) is required.");

            RuleFor(x => x.CurrentUserId)
                .NotEmpty().WithMessage("The user ID of the administrator marking the leave is required for auditing.");

            When(x => x.Dto != null, () =>
            {
                RuleFor(x => x.Dto.EmployeeId)
                    .NotEmpty().WithMessage("Employee ID is required for marking leave.");

                RuleFor(x => x.Dto.Date)
                    .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                    .When(x => x.Dto.Date.HasValue)
                    .WithMessage("Leave date cannot be in the future.");
            });
        }
    }
}