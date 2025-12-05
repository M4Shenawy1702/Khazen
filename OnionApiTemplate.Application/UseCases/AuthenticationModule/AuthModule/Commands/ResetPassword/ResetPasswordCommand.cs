using Khazen.Application.DOTs.Auth;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ResetPassword
{
    public record ResetPasswordCommand(ResetPasswordDto ResetPasswordDto) : IRequest<bool>;
}
