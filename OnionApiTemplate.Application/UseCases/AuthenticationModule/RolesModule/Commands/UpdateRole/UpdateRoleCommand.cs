namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.UpdateRole
{
    public record UpdateRoleCommand(string RoleId, string NewRoleName) : IRequest<bool>;
}
