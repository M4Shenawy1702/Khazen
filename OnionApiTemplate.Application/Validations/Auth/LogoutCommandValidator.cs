using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.LogOut;

namespace Khazen.Application.UseCases.AuthService.Commands.Logout
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

        }
    }
}
