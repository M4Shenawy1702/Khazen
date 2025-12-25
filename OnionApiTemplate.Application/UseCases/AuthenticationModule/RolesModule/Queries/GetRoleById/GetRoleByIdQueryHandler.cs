using Khazen.Application.DOTs.Auth;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Queries.GetRoleById
{
    internal class GetRoleByIdQueryHandler(
        RoleManager<ApplicationRole> roleManager,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        ILogger<GetRoleByIdQueryHandler> logger)
        : IRequestHandler<GetRoleByIdQuery, RoleDetailsDto>
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<GetRoleByIdQueryHandler> _logger = logger;

        public async Task<RoleDetailsDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executing GetRoleByIdQuery for RoleId: {RoleId}", request.Id);

            var role = await _roleManager.FindByIdAsync(request.Id);

            if (role == null)
            {
                _logger.LogWarning("Role query failed: Role with ID {RoleId} not found.", request.Id);
                throw new NotFoundException<ApplicationRole>(request.Id);
            }
            _logger.LogDebug("Target role found: {RoleName}", role.Name);

            if (role.IsDeleted)
            {
                _logger.LogWarning("Access denied: Role {RoleName} (ID: {RoleId}) is soft-deleted.", role.Name, role.Id);
                throw new NotFoundException<ApplicationRole>(request.Id);
            }

            var roleDto = _mapper.Map<RoleDetailsDto>(role);

            _logger.LogDebug("Retrieving claims for role {RoleName}.", role.Name);
            var claims = await _roleManager.GetClaimsAsync(role);
            roleDto.Claims = _mapper.Map<List<string>>(claims);
            _logger.LogDebug("Found {Count} claims.", claims.Count);

            _logger.LogDebug("Retrieving all users assigned to role {RoleName}. This may be a large operation.", role.Name);
            var users = await _userManager.GetUsersInRoleAsync(role.Name!);

            roleDto.Users = _mapper.Map<List<ApplicationUserDto>>(users);
            _logger.LogInformation("Successfully retrieved role details, including {UserCount} users and {ClaimCount} claims.",
                users.Count, claims.Count);

            return roleDto;
        }
    }
}