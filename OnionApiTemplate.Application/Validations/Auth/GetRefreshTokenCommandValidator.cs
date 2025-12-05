using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.GetRefreshToken;

namespace Khazen.Application.Validations.Auth
{
    public class GetRefreshTokenCommandValidator : AbstractValidator<GetRefreshTokenCommand>
    {
        public GetRefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}
