using Khazen.Application.DOTs.Auth;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Queries.GetRoleById
{
    public record GetRoleByIdQuery(string Id) : IRequest<RoleDetailsDto>;
}
