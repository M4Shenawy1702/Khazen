using Khazen.Application.UseCases.AuthenticationModule.Common;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
}
