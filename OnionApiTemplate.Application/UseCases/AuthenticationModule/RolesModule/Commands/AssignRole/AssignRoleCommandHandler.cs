using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.AssignRole
{
    internal class AssignRoleCommandHandler(
        RoleManager<ApplicationRole> roleManager,
        IValidator<AssignRoleCommand> validator,
        UserManager<ApplicationUser> userManager,
        ILogger<AssignRoleCommandHandler> logger)
        : IRequestHandler<AssignRoleCommand, bool>
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IValidator<AssignRoleCommand> _validator = validator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<AssignRoleCommandHandler> _logger = logger;

        public async Task<bool> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to assign RoleId: {RoleId} to UserId: {UserId}.",
                request.RoleId, request.UserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("AssignRoleCommand validation failed for UserId: {UserId}. Errors: {Errors}",
                    request.UserId, string.Join("; ", errors));
                throw new BadRequestException(string.Join(", ", errors));
            }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("Assignment failed: User with ID {UserId} not found.", request.UserId);
                throw new NotFoundException<ApplicationUser>(request.UserId);
            }
            _logger.LogDebug("User found: {UserName}", user.UserName);

            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role == null)
            {
                _logger.LogWarning("Assignment failed: Role with ID {RoleId} not found.", request.RoleId);
                throw new NotFoundException<ApplicationRole>(request.RoleId);
            }
            _logger.LogDebug("Role found: {RoleName}", role.Name);

            if (role.IsDeleted)
            {
                _logger.LogWarning("Assignment aborted: Role {RoleName} (ID: {RoleId}) is marked as deleted.",
                    role.Name, role.Id);
                throw new BadRequestException($"Cannot assign role '{role.Name}' because it is inactive.");
            }

            if (await _userManager.IsInRoleAsync(user, role.Name!))
            {
                _logger.LogWarning("Assignment skipped: User {UserName} already has role {RoleName}.",
                    user.UserName, role.Name);
                return true;
            }

            _logger.LogInformation("Assigning role {RoleName} to user {UserName}.",
                role.Name, user.UserName);

            var result = await _userManager.AddToRoleAsync(user, role.Name!);

            if (!result.Succeeded)
            {
                var identityErrors = result.Errors.Select(x => x.Description).ToList();
                _logger.LogError("Failed to assign role {RoleName} to user {UserName}. Identity Errors: {Errors}",
                    role.Name, user.UserName, string.Join("; ", identityErrors));

                throw new BadRequestException(string.Join(", ", identityErrors));
            }

            _logger.LogInformation("Successfully assigned role {RoleName} to user {UserName}.",
                role.Name, user.UserName);

            return true;
        }
    }
}