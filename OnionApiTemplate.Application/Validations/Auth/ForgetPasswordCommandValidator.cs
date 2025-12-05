using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.FogetPassword;

namespace Khazen.Application.Validations.Auth
{
    public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
    {
        public ForgetPasswordCommandValidator()
        {
            RuleFor(x => x.ForgetPasswordDto.Email)
                .NotEmpty().EmailAddress();

            RuleFor(x => x.ForgetPasswordDto.ClientUrl)
                .NotEmpty().WithMessage("Client URL is required.");
        }
    }

}
