namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.SendVerificationOtp
{
    public record SendOtpCommand(string UserId) : IRequest<bool>;
}
