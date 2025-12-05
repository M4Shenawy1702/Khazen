using Khazen.Application.DOTs.Auth;
using Microsoft.EntityFrameworkCore;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Queries.GetAllRoles
{
    internal class GetAllRolesQueryHandler(RoleManager<IdentityRole> roleManager, IMapper mapper)
        : IRequestHandler<GetAllRolesQuery, IEnumerable<RoleDto>>
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.ToListAsync(cancellationToken);

            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }
    }
}
