using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.LogOut
{
    internal class LogoutCommandHandler(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IValidator<LogoutCommand> validator,
        ILogger<LogoutCommandHandler> logger)
        : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IValidator<LogoutCommand> _validator = validator;
        private readonly ILogger<LogoutCommandHandler> _logger = logger;

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Logout request received for UserId: {UserId}", request.UserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("Logout validation failed for UserId {UserId}. Errors: {Errors}",
                    request.UserId, string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var user = await _userManager.Users
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Logout requested for non-existent or deleted UserId: {UserId}", request.UserId);
                return true;
            }

            var activeTokens = user.RefreshTokens?.Where(rt => rt.IsActive).ToList();

            if (activeTokens == null || activeTokens.Count == 0)
            {
                _logger.LogInformation("No active refresh tokens found for user {UserId}. Logout complete.", user.Id);
                return true;
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction started to revoke {Count} tokens for user {UserId}.", activeTokens.Count, user.Id);

            try
            {
                foreach (var token in activeTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                    _logger.LogTrace("Revoking token {TokenId} for user {UserId}", token.Id, user.Id);
                }

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Successfully revoked {Count} refresh tokens for user {UserId}. Transaction committed.", activeTokens.Count, user.Id);
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Failed to revoke tokens for user {UserId}. Transaction rolled back.", user.Id);
                throw;
            }
        }
    }
}