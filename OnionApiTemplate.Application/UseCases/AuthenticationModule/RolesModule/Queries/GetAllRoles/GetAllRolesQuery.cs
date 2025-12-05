using Khazen.Application.DOTs.Auth;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Queries.GetAllRoles
{
    public record GetAllRolesQuery() : IRequest<IEnumerable<RoleDto>>;

}
