using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.ObjectValues;
using Khazen.Domain.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ResendEmailVerification
{
    internal class ResendEmailVerificationCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        ILogger<ResendEmailVerificationCommandHandler> logger
    ) : IRequestHandler<ResendEmailVerificationCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly ILogger<ResendEmailVerificationCommandHandler> _logger = logger;

        public async Task<bool> Handle(ResendEmailVerificationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Resend verification request received for UserId: {UserId}", request.UserId);

            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                _logger.LogWarning("Resend verification failed: User not found for UserId: {UserId}", request.UserId);
                throw new NotFoundException<ApplicationUser>(request.UserId);
            }
            _logger.LogDebug("User found: {Email}", user.Email);

            if (string.IsNullOrEmpty(user.Email))
            {
                _logger.LogError("Resend verification failed: User {UserId} has no email address configured.", request.UserId);
                throw new BadRequestException("User does not have an email address.");
            }

            if (user.EmailConfirmed)
            {
                _logger.LogWarning("Resend verification failed: Email for user {UserId} is already verified.", request.UserId);
                throw new BadRequestException("Email is already verified.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            _logger.LogDebug("Verification token generated and encoded for user {UserId}", user.Id);

            var verificationUrl = $"{request.ClientUrl}/verify-email?userId={user.Id}&token={encodedToken}";
            _logger.LogTrace("Verification URL constructed: {Url}", verificationUrl);

            var emailBody = $@"
                <h2>Verify your email</h2>
                <p>Hello {user.UserName},</p>
                <p>Please click the link below to verify your email address:</p>
                <a href='{verificationUrl}' target='_blank'>Verify Email</a>
                <p>This link will expire soon for your security.</p>";

            var message = new EmailMessage(
                [new(user.UserName ?? user.Email, user.Email)],
                "Khazen - Email Verification",
                emailBody,
                null
            );

            await _emailSender.SendEmailAsync(message);

            _logger.LogInformation("Verification email sent successfully to {Email} for user {UserId}", user.Email, user.Id);

            return true;
        }
    }
}