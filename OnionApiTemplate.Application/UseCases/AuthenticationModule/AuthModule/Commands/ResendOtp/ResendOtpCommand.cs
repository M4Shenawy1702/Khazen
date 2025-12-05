namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ResendOtp
{
    public record ResendOtpCommand(string UserId) : IRequest<bool>;
}
