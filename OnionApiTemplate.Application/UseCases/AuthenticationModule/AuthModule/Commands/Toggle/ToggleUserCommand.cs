namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.Toggle
{
    public record ToggleUserCommand(string Id, string CurrentUserId) : IRequest<bool>;
}
