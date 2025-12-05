namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.VerifyOtp
{
    public record VerifyOtpCommand(string UserId, string Otp) : IRequest<bool>;
}
