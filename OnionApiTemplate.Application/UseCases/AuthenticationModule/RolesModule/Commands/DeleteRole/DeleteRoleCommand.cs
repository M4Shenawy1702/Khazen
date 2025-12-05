namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.DeleteRole
{
    public record DeleteRoleCommand(string RoleId) : IRequest<bool>;
}
