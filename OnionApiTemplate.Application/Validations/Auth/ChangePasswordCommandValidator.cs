using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ChangePassword;

namespace Khazen.Application.UseCases.AuthService.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.Dto)
                .NotNull().WithMessage("Password data is required.");

            RuleFor(x => x.Dto.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.")
                .MinimumLength(6).WithMessage("Current password must be at least 6 characters long.");

            RuleFor(x => x.Dto.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("New password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("New password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("New password must contain at least one special character.");
        }
    }
}
