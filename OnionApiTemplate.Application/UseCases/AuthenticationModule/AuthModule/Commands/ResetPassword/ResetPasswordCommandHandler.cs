using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging; // Added logging namespace

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ResetPassword
{
    internal class ResetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IValidator<ResetPasswordCommand> validator,
        ILogger<ResetPasswordCommandHandler> logger) // 💡 Added ILogger
        : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly ILogger<ResetPasswordCommandHandler> _logger = logger;

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Password reset request received for email: {Email}", request.ResetPasswordDto.Email);

            var validation = await validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Reset validation failed for email {Email}: {Errors}", request.ResetPasswordDto.Email, string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var user = await userManager.FindByEmailAsync(request.ResetPasswordDto.Email);

            if (user == null)
            {
                _logger.LogWarning("Reset attempt for non-existent email: {Email}. Failing silently.", request.ResetPasswordDto.Email);
                return true;
            }

            var result = await userManager.ResetPasswordAsync(user, request.ResetPasswordDto.Token, request.ResetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Password reset failed for user {UserId}. Errors: {Errors}",
                    user.Id, string.Join(", ", result.Errors.Select(e => e.Description)));

                throw new BadRequestException("The token is invalid or has expired.");
            }

            _logger.LogInformation("Password reset succeeded for user {UserId}. Revoking all tokens.", user.Id);

            var activeRefreshTokens = user.RefreshTokens!.Where(x => x.IsActive).ToList();

            foreach (var rt in activeRefreshTokens)
            {
                rt.RevokedAt = DateTime.UtcNow;
            }

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                _logger.LogError("Failed to update user/revoke tokens for user {UserId} after successful password reset. MANUAL INTERVENTION REQUIRED. Errors: {Errors}",
                    user.Id, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }
            else
            {
                _logger.LogInformation("Successfully revoked {Count} tokens for user {UserId}.", activeRefreshTokens.Count, user.Id);
            }

            return true;
        }
    }
}