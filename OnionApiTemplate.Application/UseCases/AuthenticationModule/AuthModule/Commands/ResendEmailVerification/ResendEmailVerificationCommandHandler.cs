using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.ObjectValues;
using Khazen.Domain.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ResendEmailVerification
{
    internal class ResendEmailVerificationCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender
    ) : IRequestHandler<ResendEmailVerificationCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IEmailSender _emailSender = emailSender;

        public async Task<bool> Handle(ResendEmailVerificationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new NotFoundException<ApplicationUser>(request.UserId);

            if (string.IsNullOrEmpty(user.Email))
                throw new BadRequestException("User does not have an email address.");

            if (user.EmailConfirmed)
                throw new BadRequestException("Email is already verified.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var verificationUrl = $"{request.ClientUrl}/verify-email?userId={user.Id}&token={encodedToken}";

            // Build email content
            var emailBody = $@"
                <h2>Verify your email</h2>
                <p>Hello {user.UserName},</p>
                <p>Please click the link below to verify your email address:</p>
                <a href='{verificationUrl}' target='_blank'>Verify Email</a>
                <p>This link will expire soon for your security.</p>";

            var message = new EmailMessage(
                [new(user.UserName, user.Email)],
                "Khazen - Email Verification",
                emailBody,
                null
            );

            await _emailSender.SendEmailAsync(message);

            return true;
        }
    }
}
