namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.SendEmailVerification
{
    public record SendEmailVerificationCommand(string UserId, string ClientUrl) : IRequest<bool>;
}
