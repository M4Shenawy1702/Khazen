namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.VerifyEmail
{
    public record VerifyEmailCommand(string UserId, string Token) : IRequest<bool>;
}
