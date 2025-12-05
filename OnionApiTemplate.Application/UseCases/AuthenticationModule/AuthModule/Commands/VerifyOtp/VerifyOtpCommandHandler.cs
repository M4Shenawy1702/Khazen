using Khazen.Application.Common.Interfaces;
using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.VerifyOtp
{
    internal class VerifyOtpCommandHandler(UserManager<ApplicationUser> userManager, ISmsService smsService)
        : IRequestHandler<VerifyOtpCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ISmsService _smsService = smsService;
        public async Task<bool> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new NotFoundException<ApplicationUser>(request.UserId);

            if (string.IsNullOrEmpty(user.PhoneNumber))
                throw new BadRequestException("User does not have a phone number.");

            if (user.PhoneNumberConfirmed)
                throw new BadRequestException("User phone number is already confirmed.");

            if (user.PhoneOtpExpiry < DateTime.UtcNow)
                throw new BadRequestException("OTP has expired, please request a new one.");

            if (user.PhoneOtpCode != request.Otp)
                throw new BadRequestException("OTP does not match, please try again.");

            user.PhoneNumberConfirmed = true;
            user.PhoneOtpCode = null;
            user.PhoneOtpExpiry = null;
            await _userManager.UpdateAsync(user);

            return true;
        }
    }
}
