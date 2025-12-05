using Khazen.Application.UseCases.AuthenticationModule.Common;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.GetRefreshToken
{
    public record GetRefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;
}
