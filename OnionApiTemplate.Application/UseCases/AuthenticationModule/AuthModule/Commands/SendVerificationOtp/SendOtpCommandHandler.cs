using Khazen.Application.Common.Configurations;
using Khazen.Application.Common.Factories;
using Khazen.Application.Common.ObjectValues;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.SendVerificationOtp
{
    internal class SendOtpCommandHandler(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IOptions<TwilioSmsConfigurations> twilioOptions,
        ILogger<SendOtpCommandHandler> logger
    ) : IRequestHandler<SendOtpCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly IOptions<TwilioSmsConfigurations> _twilioOptions = twilioOptions;
        private readonly ILogger<SendOtpCommandHandler> _logger = logger;

        private const int OTP_RATE_LIMIT_MINUTES = 1;
        private const int OTP_EXPIRY_MINUTES = 5;

        public async Task<bool> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("OTP send request received for UserId: {UserId}", request.UserId);

            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                _logger.LogWarning("OTP send request received for non-existent UserId: {UserId}. Failing softly.", request.UserId);
                return true;
            }
            _logger.LogDebug("User found. Phone: {PhoneNumber}", user.PhoneNumber);


            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                _logger.LogError("OTP send failed: User {UserId} has no phone number configured.", request.UserId);
                throw new BadRequestException("User does not have a phone number.");
            }

            if (user.PhoneNumberConfirmed)
            {
                _logger.LogWarning("OTP send skipped: Phone number for user {UserId} is already verified.", request.UserId);
                throw new BadRequestException("User phone number is already verified.");
            }

            if (user.PhoneOtpExpiry.HasValue && user.PhoneOtpExpiry.Value > DateTime.UtcNow.AddMinutes(-OTP_RATE_LIMIT_MINUTES))
            {
                var waitTime = user.PhoneOtpExpiry.Value.Subtract(DateTime.UtcNow.AddMinutes(-OTP_RATE_LIMIT_MINUTES));
                _logger.LogWarning("Resend OTP rate limit active for user {UserId}. Must wait {WaitTime} more.", user.Id, waitTime);
                throw new BadRequestException($"Please wait {waitTime.Seconds} seconds before requesting a new OTP.");
            }

            var codeBytes = RandomNumberGenerator.GetBytes(4);
            var codeInt = BitConverter.ToInt32(codeBytes, 0) % 1000000;
            var code = codeInt.ToString("D6");

            user.PhoneOtpCode = code;
            user.PhoneOtpExpiry = DateTime.UtcNow.AddMinutes(OTP_EXPIRY_MINUTES);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogError("Failed to update OTP state for user {UserId}. Errors: {Errors}",
                    user.Id, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                throw new InvalidOperationException("Failed to update user's OTP state.");
            }
            _logger.LogDebug("New secure OTP code generated and user state updated for user {UserId}.", user.Id);


            var smsProviderType = _configuration["SmsSettings:Provider"] ?? "Mock";
            _logger.LogTrace("Attempting to create SMS service provider: {Provider}", smsProviderType);

            var smsService = SMSFactory.CreateAccount(smsProviderType, _twilioOptions);

            var smsMessage = new SMSMessage(user.PhoneNumber, $"Your Khazen verification code is: {code}");
            await smsService.SendSmsAsync(smsMessage);

            _logger.LogInformation("OTP sent successfully via {Provider} to phone number for user {UserId}", smsProviderType, user.Id);

            return true;
        }
    }
}