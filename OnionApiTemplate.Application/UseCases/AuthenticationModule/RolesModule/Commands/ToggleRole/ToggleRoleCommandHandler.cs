using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.DeleteRole
{
    internal class ToggleRoleCommandHandler(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<ToggleRoleCommandHandler> logger)
        : IRequestHandler<ToggleRoleCommand, bool>
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<ToggleRoleCommandHandler> _logger = logger;

        public async Task<bool> Handle(ToggleRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing Role Status Toggle for RoleId: {RoleId}, Requested By: {ToggeledBy}",
                request.RoleId, request.ToggeledBy);

            var creatorUser = await _userManager.FindByNameAsync(request.ToggeledBy);

            if (creatorUser == null)
            {
                _logger.LogWarning("Toggle failed: Creator user '{ToggeledBy}' not found.", request.ToggeledBy);
                throw new NotFoundException($"Creator user '{request.ToggeledBy}' not found.");
            }
            string creatorUsername = creatorUser.UserName ?? request.ToggeledBy;

            var role = await _roleManager.FindByIdAsync(request.RoleId);

            if (role == null)
            {
                _logger.LogWarning("Toggle failed: Role with ID {RoleId} not found.", request.RoleId);
                throw new NotFoundException<ApplicationRole>(request.RoleId);
            }

            role.IsDeleted = !role.IsDeleted;
            role.ToggeledBy = creatorUsername;
            role.ToggeledAt = DateTime.UtcNow;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                var identityErrors = result.Errors.Select(x => x.Description).ToList();
                _logger.LogError("Update (Toggle) of role {RoleName} failed. Identity Errors: {Errors}",
                    role.Name, string.Join("; ", identityErrors));

                throw new BadRequestException(string.Join(", ", identityErrors));
            }

            _logger.LogInformation("Role {RoleName} successfully toggled.", role.Name);

            return true;
        }
    }
}