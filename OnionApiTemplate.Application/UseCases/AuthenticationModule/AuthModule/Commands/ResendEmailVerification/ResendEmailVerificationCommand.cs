namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ResendEmailVerification
{
    public record ResendEmailVerificationCommand(string UserId, string ClientUrl) : IRequest<bool>;
}
