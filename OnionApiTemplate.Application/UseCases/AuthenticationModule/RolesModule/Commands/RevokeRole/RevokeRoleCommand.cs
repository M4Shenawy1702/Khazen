namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.RevokeRole
{
    public record RevokeRoleCommand(string UserId, string RoleId) : IRequest<bool>;
}
