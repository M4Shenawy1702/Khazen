using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.ObjectValues;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ResendOtp
{
    internal class ResendOtpCommandHandler(
        UserManager<ApplicationUser> userManager,
        ISmsService smsService,
        ILogger<ResendOtpCommandHandler> logger
    ) : IRequestHandler<ResendOtpCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ISmsService _smsService = smsService;
        private readonly ILogger<ResendOtpCommandHandler> _logger = logger;

        public async Task<bool> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Resend OTP request received for UserId: {UserId}", request.UserId);

            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                _logger.LogWarning("Resend OTP failed: User not found for UserId: {UserId}", request.UserId);
                throw new NotFoundException<ApplicationUser>(request.UserId);
            }
            _logger.LogDebug("User found. Attempting to resend OTP to phone: {PhoneNumber}", user.PhoneNumber);

            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                _logger.LogError("Resend OTP failed: User {UserId} has no phone number configured.", request.UserId);
                throw new BadRequestException("User does not have a phone number.");
            }

            if (user.PhoneNumberConfirmed)
            {
                _logger.LogWarning("Resend OTP failed: Phone number for user {UserId} is already verified.", request.UserId);
                throw new BadRequestException("User phone number is already verified.");
            }

            if (user.PhoneOtpExpiry > DateTime.UtcNow.AddMinutes(-1))
            {
                var waitTime = user.PhoneOtpExpiry.Value.Subtract(DateTime.UtcNow.AddMinutes(-1));
                _logger.LogWarning("Resend OTP rate limit active for user {UserId}. Must wait {WaitTime} more.", user.Id, waitTime);
                throw new BadRequestException("Please wait before requesting a new OTP.");
            }

            var newCode = new Random().Next(100000, 999999).ToString();

            user.PhoneOtpCode = newCode;
            user.PhoneOtpExpiry = DateTime.UtcNow.AddMinutes(5);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogError("Failed to update OTP code/expiry for user {UserId}. Errors: {Errors}",
                    user.Id, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                throw new InvalidOperationException("Failed to update user's OTP state.");
            }
            _logger.LogDebug("New OTP code generated and user state updated for user {UserId}.", user.Id);

            var smsMessage = new SMSMessage(user.PhoneNumber, $"Your new Khazen verification code is: {newCode}");
            await _smsService.SendSmsAsync(smsMessage);

            _logger.LogInformation("New OTP sent successfully to phone number for user {UserId}", user.Id);

            return true;
        }
    }
}