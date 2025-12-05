namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.LogOut
{
    public record LogoutCommand(string UserId) : IRequest<bool>;
}
