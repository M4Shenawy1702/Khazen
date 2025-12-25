using Khazen.Domain.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.VerifyEmail
{
    internal class VerifyEmailCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<VerifyEmailCommandHandler> logger)
        : IRequestHandler<VerifyEmailCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<VerifyEmailCommandHandler> _logger = logger;

        public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Email verification request received for UserId: {UserId}", request.UserId);

            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                _logger.LogWarning("Email verification failed: User not found for UserId: {UserId}", request.UserId);
                throw new NotFoundException<ApplicationUser>(request.UserId);
            }
            _logger.LogDebug("User found: {Email}", user.Email);

            string decodedToken;
            try
            {
                var tokenBytes = WebEncoders.Base64UrlDecode(request.Token);
                decodedToken = Encoding.UTF8.GetString(tokenBytes);
                _logger.LogTrace("Token successfully decoded.");
            }
            catch (FormatException ex)
            {
                _logger.LogWarning(ex, "Token decoding failed (bad format) for UserId: {UserId}", user.Id);
                throw new BadRequestException("Invalid or malformed verification token.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                _logger.LogWarning("Email confirmation failed for user {UserId}. Errors: {Errors}",
                    user.Id, string.Join(", ", errors));

                throw new BadRequestException("Invalid or expired email verification token.");
            }

            _logger.LogInformation("Email successfully confirmed for user {UserId}.", user.Id);
            return true;
        }
    }
}