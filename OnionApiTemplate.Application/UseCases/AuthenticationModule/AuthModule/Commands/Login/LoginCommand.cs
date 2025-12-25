using Khazen.Application.DOTs.Auth;
using Khazen.Application.UseCases.AuthenticationModule.Common;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.Login
{
    public record LoginCommand(LoginRequestDto Dto) : IRequest<AuthResponse>;
}
