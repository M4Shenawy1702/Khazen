using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.ObjectValues;
using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ResendOtp
{
    internal class ResendOtpCommandHandler(
        UserManager<ApplicationUser> userManager,
        ISmsService smsService
    ) : IRequestHandler<ResendOtpCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ISmsService _smsService = smsService;

        public async Task<bool> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new NotFoundException<ApplicationUser>(request.UserId);

            if (string.IsNullOrEmpty(user.PhoneNumber))
                throw new BadRequestException("User does not have a phone number.");

            if (user.PhoneNumberConfirmed)
                throw new BadRequestException("User phone number is already verified.");

            if (user.PhoneOtpExpiry > DateTime.UtcNow.AddMinutes(-1))
                throw new BadRequestException("Please wait before requesting a new OTP.");

            var newCode = new Random().Next(100000, 999999).ToString();

            user.PhoneOtpCode = newCode;
            user.PhoneOtpExpiry = DateTime.UtcNow.AddMinutes(5);

            await _userManager.UpdateAsync(user);

            var smsMessage = new SMSMessage(user.PhoneNumber, $"Your new Khazen verification code is: {newCode}");
            await _smsService.SendSmsAsync(smsMessage);

            return true;
        }
    }
}
