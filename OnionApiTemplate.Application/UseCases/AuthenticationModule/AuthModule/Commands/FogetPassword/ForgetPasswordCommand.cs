using Khazen.Application.DOTs.Auth;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.FogetPassword
{
    public record ForgetPasswordCommand(ForgetPasswordDto ForgetPasswordDto) : IRequest<bool>;
}
