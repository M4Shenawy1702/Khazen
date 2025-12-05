namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.AddRole
{
    public record CreateRoleCommand(string RoleName) : IRequest<bool>;
}
