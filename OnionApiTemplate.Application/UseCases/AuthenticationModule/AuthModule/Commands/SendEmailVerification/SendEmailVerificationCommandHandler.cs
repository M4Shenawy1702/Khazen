using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.ObjectValues;
using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.SendEmailVerification
{
    internal class SendEmailVerificationCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender)
        : IRequestHandler<SendEmailVerificationCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IEmailSender _emailSender = emailSender;

        public async Task<bool> Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new NotFoundException<ApplicationUser>(request.UserId);

            if (user.EmailConfirmed)
                throw new BadRequestException("Email is already verified.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var verificationLink = $"{request.ClientUrl}/verify-email?userId={user.Id}&token={encodedToken}";

            var message = new EmailMessage(
                [new(user.Email, user.Email)],
                "Verify Your Email",
                $"""
                <h2>Welcome to Khazen System</h2>
                <p>Please verify your email by clicking the link below:</p>
                <a href="{verificationLink}">Verify Email</a>
                """,
                null
            );

            await _emailSender.SendEmailAsync(message);
            return true;
        }
    }
}
