using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.AddRole
{
    internal class CreateRoleCommandHandler(
        RoleManager<ApplicationRole> roleManager,
        IValidator<CreateRoleCommand> validator,
        ILogger<CreateRoleCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<CreateRoleCommand, bool>
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IValidator<CreateRoleCommand> _validator = validator;
        private readonly ILogger<CreateRoleCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to create new role: {RoleName}. Creator: {CreatedBy}",
                request.RoleName, request.CreatedBy);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("Role creation failed validation for role {RoleName}. Errors: {Errors}",
                    request.RoleName, string.Join("; ", errors));
                throw new BadRequestException(errors);
            }

            var creatorUser = await _userManager.FindByNameAsync(request.CreatedBy);

            if (creatorUser == null)
            {
                _logger.LogWarning("Role creation failed: Creator user '{CreatedBy}' not found.", request.CreatedBy);
                throw new NotFoundException($"Creator user '{request.CreatedBy}' not found.");
            }

            var normalizedRoleName = _roleManager.NormalizeKey(request.RoleName);
            var existingRole = await _roleManager.FindByNameAsync(normalizedRoleName);

            if (existingRole != null)
            {
                _logger.LogWarning("Role creation failed: Role {RoleName} already exists.", request.RoleName);
                throw new BadRequestException($"Role '{request.RoleName}' already exists.");
            }

            var newRole = new ApplicationRole
            {
                Name = request.RoleName,
                NormalizedName = normalizedRoleName,
                CreatedBy = creatorUser.Id
            };

            _logger.LogDebug("Creating role entity for {RoleName}", request.RoleName);
            var result = await _roleManager.CreateAsync(newRole);

            if (!result.Succeeded)
            {
                var identityErrors = result.Errors.Select(x => x.Description).ToList();
                _logger.LogError("Failed to create role {RoleName}. Identity Errors: {Errors}",
                    request.RoleName, string.Join("; ", identityErrors));
                throw new BadRequestException(identityErrors);
            }

            _logger.LogInformation("Successfully created new role with ID: {RoleId}, Name: {RoleName} by User ID: {CreatorId}",
                newRole.Id, newRole.Name, creatorUser.Id);

            return true;
        }
    }
}