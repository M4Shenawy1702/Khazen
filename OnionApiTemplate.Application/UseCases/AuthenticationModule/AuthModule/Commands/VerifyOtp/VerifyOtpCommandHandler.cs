using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.VerifyOtp
{
    internal class VerifyOtpCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILogger<VerifyOtpCommandHandler> logger)
        : IRequestHandler<VerifyOtpCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<VerifyOtpCommandHandler> _logger = logger;

        private const int MAX_OTP_ATTEMPTS = 5;

        public async Task<bool> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("OTP verification request received for UserId: {UserId}", request.UserId);

            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                _logger.LogWarning("OTP verification failed: User not found for UserId: {UserId}. Failing softly.", request.UserId);
                return true;
            }
            _logger.LogDebug("User found. Attempting to verify phone number.");

            int currentAttempts = user.PhoneOtpFailedAttempts ?? 0;


            if (string.IsNullOrEmpty(user.PhoneNumber) || user.PhoneNumberConfirmed)
            {
                _logger.LogWarning("OTP verification failed for user {UserId} due to invalid state (null/confirmed phone).", user.Id);
                throw new BadRequestException("The phone number is already confirmed or missing.");
            }

            if (user.PhoneOtpExpiry < DateTime.UtcNow)
            {
                _logger.LogWarning("OTP verification failed for user {UserId}: Token expired at {ExpiryTime}. Cleared OTP state.", user.Id, user.PhoneOtpExpiry);
                user.PhoneOtpCode = null;
                user.PhoneOtpExpiry = null;
                await _userManager.UpdateAsync(user);
                throw new BadRequestException("OTP has expired, please request a new one.");
            }

            if (currentAttempts >= MAX_OTP_ATTEMPTS)
            {
                _logger.LogError("OTP verification failed for user {UserId}: Brute force threshold exceeded.", user.Id);
                user.PhoneOtpCode = null;
                user.PhoneOtpExpiry = null;
                await _userManager.UpdateAsync(user);
                throw new BadRequestException("Too many failed attempts. Please request a new OTP.");
            }


            if (user.PhoneOtpCode != request.Otp)
            {
                user.PhoneOtpFailedAttempts = currentAttempts + 1;
                await _userManager.UpdateAsync(user);

                _logger.LogWarning("OTP verification failed for user {UserId}: OTP mismatch. Attempt {Attempt}/{MaxAttempts}", user.Id, user.PhoneOtpFailedAttempts, MAX_OTP_ATTEMPTS);
                throw new BadRequestException("OTP does not match, please try again.");
            }

            _logger.LogInformation("OTP successfully verified for user {UserId}.", user.Id);

            user.PhoneNumberConfirmed = true;
            user.PhoneOtpCode = null;
            user.PhoneOtpExpiry = null;
            user.PhoneOtpFailedAttempts = 0; // Reset counter on success

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                _logger.LogError("Failed to save phone verification status for {UserId}. Errors: {Errors}",
                    user.Id, string.Join(", ", errors));
                throw new InvalidOperationException("Failed to save phone verification status.");
            }

            _logger.LogInformation("User {UserId}'s phone number is now confirmed.", user.Id);

            return true;
        }
    }
}