using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.ObjectValues;
using Khazen.Domain.Exceptions;
using Microsoft.AspNetCore.WebUtilities; // Needed for correct encoding
using Microsoft.Extensions.Logging;
using System.Text; // Needed for encoding

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.SendEmailVerification
{
    internal class SendEmailVerificationCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        ILogger<SendEmailVerificationCommandHandler> logger) // 💡 Added ILogger
        : IRequestHandler<SendEmailVerificationCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly ILogger<SendEmailVerificationCommandHandler> _logger = logger;

        public async Task<bool> Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Send Email Verification request received for UserId: {UserId}", request.UserId);

            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                _logger.LogWarning("Verification email failed: User not found for UserId: {UserId}", request.UserId);
                throw new NotFoundException<ApplicationUser>(request.UserId);
            }
            _logger.LogDebug("User found: {Email}", user.Email);

            if (user.EmailConfirmed)
            {
                _logger.LogWarning("Verification email skipped: Email for user {UserId} is already confirmed.", request.UserId);
                throw new BadRequestException("Email is already verified.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            _logger.LogDebug("Verification token generated and encoded for user {UserId}", user.Id);

            var verificationLink = $"{request.ClientUrl}/verify-email?userId={user.Id}&token={encodedToken}";
            _logger.LogTrace("Verification URL constructed: {Url}", verificationLink);

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

            _logger.LogInformation("Verification email sent successfully to {Email} for user {UserId}", user.Email, user.Id);
            return true;
        }
    }
}