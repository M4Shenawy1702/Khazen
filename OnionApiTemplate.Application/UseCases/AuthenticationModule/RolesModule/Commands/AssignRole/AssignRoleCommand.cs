namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.AssignRole
{
    public record AssignRoleCommand(string UserId, string RoleId) : IRequest<bool>;
}
