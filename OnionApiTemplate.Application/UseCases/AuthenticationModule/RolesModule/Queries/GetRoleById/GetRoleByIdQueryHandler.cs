using Khazen.Application.DOTs.Auth;
using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Queries.GetRoleById
{
    internal class GetRoleByIdQueryHandler(RoleManager<IdentityRole> roleManager, IMapper mapper, UserManager<ApplicationUser> userManager)
        : IRequestHandler<GetRoleByIdQuery, RoleDetailsDto>
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<RoleDetailsDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.Id)
                ?? throw new NotFoundException<IdentityRole>(request.Id);

            var roleDto = _mapper.Map<RoleDetailsDto>(role);

            var claims = await _roleManager.GetClaimsAsync(role);
            roleDto.Claims = _mapper.Map<List<string>>(claims);

            var users = await _userManager.GetUsersInRoleAsync(role.Name!);
            roleDto.Users = _mapper.Map<List<ApplicationUserDto>>(users);

            return roleDto;
        }
    }
}
