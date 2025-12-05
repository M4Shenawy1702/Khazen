using Khazen.Application.DOTs.Auth;
using Khazen.Application.UseCases.AuthenticationModule.Common;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ChangePassword
{
    public record ChangePasswordCommand(string UserId, ChangePasswordDto Dto) : IRequest<AuthResponse>;
}
