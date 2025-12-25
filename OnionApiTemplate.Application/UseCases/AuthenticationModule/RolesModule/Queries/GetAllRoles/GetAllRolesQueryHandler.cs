using Khazen.Application.DOTs.Auth;
using Khazen.Application.UseCases.AuthService.RolesModule.Queries.GetAllRoles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthenticationModule.RolesModule.Queries.GetAllRoles
{
    internal class GetAllRolesQueryHandler(
        RoleManager<ApplicationRole> roleManager,
        IMapper mapper,
        ILogger<GetAllRolesQueryHandler> logger)
        : IRequestHandler<GetAllRolesQuery, IEnumerable<RoleDto>>
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllRolesQueryHandler> _logger = logger;

        public async Task<IEnumerable<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executing GetAllRolesQuery.");

            var rolesQuery = _roleManager.Roles
                .Where(r => !r.IsDeleted);

            _logger.LogDebug("Fetching non-deleted roles from the database.");
            var roles = await rolesQuery.ToListAsync(cancellationToken);

            _logger.LogInformation("Successfully retrieved {Count} roles.", roles.Count);

            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }
    }
}