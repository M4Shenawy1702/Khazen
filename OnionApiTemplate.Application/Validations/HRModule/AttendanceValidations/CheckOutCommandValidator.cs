using Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.CheckOut;

namespace Khazen.Application.Validations.HRModule.AttendanceValidations
{
    public class CheckOutCommandValidator : AbstractValidator<CheckOutCommand>
    {
        public CheckOutCommandValidator()
        {
            RuleFor(x => x.Dto)
                .NotNull().WithMessage("Check out data (DTO) is required.");

            RuleFor(x => x.CurrentUserId)
                .NotEmpty().WithMessage("The current user's ID is required for auditing the check-out action.");

            When(x => x.Dto != null, () =>
            {
                RuleFor(x => x.Dto.EmployeeId)
                    .NotEmpty().WithMessage("Employee ID is required for check-out.");

                RuleFor(x => x.Dto.CheckOutTime)
                     .Must(time => time <= TimeOnly.FromDateTime(DateTime.UtcNow))
                     .When(x => x.Dto.CheckOutTime.HasValue)
                     .WithMessage("Check-out time cannot be in the future.");
            });
        }
    }
}