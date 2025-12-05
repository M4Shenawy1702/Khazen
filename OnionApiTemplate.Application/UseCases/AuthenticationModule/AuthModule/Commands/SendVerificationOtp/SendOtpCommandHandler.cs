using Khazen.Application.Common.Configurations;
using Khazen.Application.Common.Factories;
using Khazen.Application.Common.ObjectValues;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.SendVerificationOtp
{
    internal class SendOtpCommandHandler(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IOptions<TwilioSmsConfigurations> twilioOptions
    ) : IRequestHandler<SendOtpCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly IOptions<TwilioSmsConfigurations> _twilioOptions = twilioOptions;

        public async Task<bool> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new NotFoundException<ApplicationUser>(request.UserId);

            if (string.IsNullOrEmpty(user.PhoneNumber))
                throw new BadRequestException("User does not have a phone number.");

            var code = new Random().Next(100000, 999999).ToString();
            user.PhoneOtpCode = code;
            user.PhoneOtpExpiry = DateTime.UtcNow.AddMinutes(5);
            await _userManager.UpdateAsync(user);

            var smsProviderType = _configuration["SmsSettings:Provider"] ?? "Mock";
            var smsService = SMSFactory.CreateAccount(smsProviderType, _twilioOptions);

            var smsMessage = new SMSMessage(user.PhoneNumber, $"Your Khazen verification code is: {code}");
            await smsService.SendSmsAsync(smsMessage);

            return true;
        }
    }
}
