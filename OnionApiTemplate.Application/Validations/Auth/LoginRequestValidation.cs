using FluentValidation;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.Login;

namespace Khazen.Application.Validations.Auth
{
    public class LoginRequestValidation : AbstractValidator<LoginCommand>
    {
        public LoginRequestValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }
}
