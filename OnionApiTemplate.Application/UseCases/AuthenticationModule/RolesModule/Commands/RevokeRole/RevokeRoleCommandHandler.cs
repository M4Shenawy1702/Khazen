using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.RevokeRole
{
    internal class RevokeRoleCommandHandler(
        RoleManager<ApplicationRole> roleManager,
        IValidator<RevokeRoleCommand> validator,
        UserManager<ApplicationUser> userManager,
        ILogger<RevokeRoleCommandHandler> logger)
        : IRequestHandler<RevokeRoleCommand, bool>
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IValidator<RevokeRoleCommand> _validator = validator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<RevokeRoleCommandHandler> _logger = logger;

        public async Task<bool> Handle(RevokeRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to revoke RoleId: {RoleId} from UserId: {UserId}.",
                request.RoleId, request.UserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("RevokeRoleCommand validation failed for UserId: {UserId}. Errors: {Errors}",
                    request.UserId, string.Join("; ", errors));
                throw new BadRequestException(string.Join(", ", errors));
            }
            _logger.LogDebug("Validation passed for role revocation command.");

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("RevokeRoleCommand failed: User with ID {UserId} not found.", request.UserId);
                throw new NotFoundException<ApplicationUser>(request.UserId);
            }
            _logger.LogDebug("User found: {UserName}", user.UserName);

            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role == null)
            {
                _logger.LogWarning("RevokeRoleCommand failed: Role with ID {RoleId} not found.", request.RoleId);
                throw new NotFoundException<ApplicationRole>(request.RoleId);
            }
            _logger.LogDebug("Role found: {RoleName}", role.Name);

            if (!await _userManager.IsInRoleAsync(user, role.Name!))
            {
                _logger.LogWarning("Revocation aborted: User {UserName} is not currently assigned to role {RoleName}.",
                    user.UserName, role.Name);
                throw new BadRequestException($"User '{user.UserName}' is not assigned to the role '{role.Name}'.");
            }

            _logger.LogInformation("Revoking role {RoleName} from user {UserName} (ID: {UserId}).",
                role.Name, user.UserName, user.Id);

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);

            if (!result.Succeeded)
            {
                var identityErrors = result.Errors.Select(e => e.Description).ToList();
                _logger.LogError("Failed to revoke role {RoleName} from user {UserName}. Identity Errors: {Errors}",
                    role.Name, user.UserName, string.Join("; ", identityErrors));

                throw new BadRequestException(string.Join("; ", identityErrors));
            }

            _logger.LogInformation("Successfully revoked role {RoleName} from user {UserName}.",
                role.Name, user.UserName);

            return true;
        }
    }
}