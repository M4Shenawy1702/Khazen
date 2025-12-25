namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.DeleteRole
{
    public record ToggleRoleCommand(string RoleId, string ToggeledBy) : IRequest<bool>;
}
