using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.UpdateRole
{
    internal class UpdateRoleCommandHandler(
        RoleManager<ApplicationRole> roleManager,
        IValidator<UpdateRoleCommand> validator,
        ILogger<UpdateRoleCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<UpdateRoleCommand, bool>
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IValidator<UpdateRoleCommand> _validator = validator;
        private readonly ILogger<UpdateRoleCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;


        public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to update Role ID: {RoleId}. New Name: {NewRoleName}.",
                request.RoleId, request.NewRoleName);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("UpdateRoleCommand validation failed. Errors: {Errors}", string.Join("; ", errors));
                throw new BadRequestException(string.Join(", ", errors));
            }

            var existingRole = await _roleManager.FindByIdAsync(request.RoleId);

            if (existingRole == null)
            {
                _logger.LogWarning("Role update failed: Role with ID {RoleId} not found.", request.RoleId);
                throw new NotFoundException<ApplicationRole>(request.RoleId);
            }

            var updaterUser = await _userManager.FindByNameAsync(request.ModifiedBy);
            if (updaterUser == null)
            {
                _logger.LogWarning("Role update failed: Updater user '{Username}' not found.", request.ModifiedBy);
                throw new NotFoundException($"Updater user '{request.ModifiedBy}' not found.");
            }

            var roleWithSameName = await _roleManager.FindByNameAsync(request.NewRoleName.Trim());

            if (roleWithSameName != null && roleWithSameName.Id != existingRole.Id)
            {
                _logger.LogWarning("Role update failed: Name '{NewRoleName}' is already in use by Role ID {ExistingRoleId}.",
                    request.NewRoleName, roleWithSameName.Id);
                throw new BadRequestException($"There is already a role with the name {request.NewRoleName}");
            }

            string normalizedNewName = _roleManager.NormalizeKey(request.NewRoleName);

            if (existingRole.Name != request.NewRoleName)
            {
                _logger.LogInformation("Renaming role '{OldName}' to '{NewName}'.", existingRole.Name, request.NewRoleName);
                existingRole.Name = request.NewRoleName;
                existingRole.NormalizedName = normalizedNewName;
            }
            else
            {
                _logger.LogDebug("Role name did not change. Only auditing fields will be updated.");
            }

            existingRole.ModifiedAt = DateTime.UtcNow;
            existingRole.ModifiedBy = request.ModifiedBy;

            var result = await _roleManager.UpdateAsync(existingRole);

            if (!result.Succeeded)
            {
                var identityErrors = result.Errors.Select(x => x.Description).ToList();
                _logger.LogError("Failed to update role {RoleName} (ID: {RoleId}). Identity Errors: {Errors}",
                    existingRole.Name, existingRole.Id, string.Join("; ", identityErrors));
                throw new BadRequestException(string.Join(", ", identityErrors));
            }

            _logger.LogInformation("Successfully updated role ID: {RoleId} to name: {NewRoleName}.",
                existingRole.Id, existingRole.Name);

            return true;
        }
    }
}