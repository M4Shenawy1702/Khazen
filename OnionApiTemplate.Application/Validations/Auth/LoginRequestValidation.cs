using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.Login;

namespace Khazen.Application.Validations.Auth
{
    public class LoginRequestValidation : AbstractValidator<LoginCommand>
    {
        private const int MinimumPasswordLength = 8;
        public LoginRequestValidation()
        {
            RuleFor(x => x.Dto.Email)
                .NotEmpty()
                    .WithErrorCode("AUTH_LOGIN_001")
                    .WithMessage("Email address is required.")

                .Must(email => !string.IsNullOrWhiteSpace(email) && email.Trim().Length > 0)
                    .WithMessage("Email address cannot be only whitespace.")
                    .WithErrorCode("AUTH_LOGIN_002")

                .EmailAddress()
                    .WithErrorCode("AUTH_LOGIN_003")
                    .WithMessage("The provided email address is not in a valid format.");

            RuleFor(x => x.Dto.Password)
                .NotEmpty()
                    .WithErrorCode("AUTH_LOGIN_004")
                    .WithMessage("Password is required.")

                .MinimumLength(MinimumPasswordLength)
                    .WithErrorCode("AUTH_LOGIN_005")
                    .WithMessage($"Password must be at least {MinimumPasswordLength} characters long.");
            RuleFor(x => x.Dto.RecaptchaToken)
                .NotEmpty()
                    .WithErrorCode("AUTH_LOGIN_006")
                    .WithMessage("Security validation token (reCAPTCHA) is required.");
        }
    }
}