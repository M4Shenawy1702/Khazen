using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.Toggle
{
    internal class ToggleUserHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<ToggleUserHandler> logger)
        : IRequestHandler<ToggleUserCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<ToggleUserHandler> _logger = logger;

        public async Task<bool> Handle(ToggleUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Toggle user status request received for UserId: {UserId}", request.Id);

            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Toggle failed: User not found for UserId: {UserId}", request.Id);
                throw new NotFoundException<ApplicationUser>(request.Id);
            }

            _logger.LogDebug("Current user state is Active={IsActive}. Toggling to Active={NewState}",
                user.IsActive, !user.IsActive);

            bool wasActive = user.IsActive;
            bool willBeActive = !user.IsActive;

            user.IsActive = willBeActive;

            if (wasActive && !willBeActive)
            {
                var activeTokens = user.RefreshTokens?.Where(rt => rt.IsActive).ToList();
                if (activeTokens != null && activeTokens.Count > 0)
                {
                    foreach (var rt in activeTokens)
                    {
                        rt.RevokedAt = DateTime.UtcNow;
                    }
                    _logger.LogInformation("Deactivating user {UserId}. Revoked {Count} active refresh tokens.", user.Id, activeTokens.Count);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);
                _logger.LogError("Failed to update user status/tokens for {UserId}. Errors: {Errors}",
                    user.Id, string.Join(", ", errors));

                throw new BadRequestException(string.Join(", ", errors));
            }

            _logger.LogInformation("User {UserId} status successfully toggled to Active={IsActive}.", user.Id, user.IsActive);

            return true;
        }
    }
}