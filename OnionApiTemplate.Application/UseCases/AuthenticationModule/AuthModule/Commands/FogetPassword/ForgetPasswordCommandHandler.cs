using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.Interfaces.Authentication;
using Khazen.Application.Common.ObjectValues;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.FogetPassword
{
    internal class ForgetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IValidator<ForgetPasswordCommand> validator,
        ILogger<ForgetPasswordCommandHandler> logger,
        IRecaptchaService recaptchaService)
        : IRequestHandler<ForgetPasswordCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IValidator<ForgetPasswordCommand> _validator = validator;
        private readonly ILogger<ForgetPasswordCommandHandler> _logger = logger;
        private readonly IRecaptchaService _recaptchaService = recaptchaService;

        public async Task<bool> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Password reset requested for email: {Email}", request.ForgetPasswordDto.Email);

            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Validation failed for password reset for email {Email}", request.ForgetPasswordDto.Email);
                throw new BadRequestException(validation.Errors.Select(e => e.ErrorMessage).ToList());
            }
            if (request.ForgetPasswordDto.RecaptchaToken is null)
            {
                _logger.LogWarning("Recaptcha token was missing but required for email: {Email}", request.ForgetPasswordDto.Email);
                throw new BadRequestException("Recaptcha token is required.");
            }

            _logger.LogInformation("Validating Recaptcha token for email: {Email}", request.ForgetPasswordDto.Email);

            var recaptchaResult = await _recaptchaService.VerifyAsync(request.ForgetPasswordDto.RecaptchaToken, request.ForgetPasswordDto.Email);

            _logger.LogInformation("Recaptcha validation passed (Score: {Score}) for email: {Email}",
                recaptchaResult.Score, request.ForgetPasswordDto.Email);


            var user = await _userManager.FindByEmailAsync(request.ForgetPasswordDto.Email);

            if (user is null)
            {
                _logger.LogInformation("Password reset request received for unknown email {Email}. Returning success to prevent enumeration.",
                    request.ForgetPasswordDto.Email);
                return true;
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Password reset requested for unconfirmed email {Email}. Failing silently.", user.Email);
                return true;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);
            var userId = user.Id;

            var resetLink = $"{request.ForgetPasswordDto.ClientUrl}/reset-password?userId={userId}&token={encodedToken}";

            var message = new EmailMessage(
                new List<MailboxAddress> { new MailboxAddress(user.UserName ?? user.Email, user.Email!) },
                "Password Reset Request",
                $@"<h2>Hello {user.UserName ?? user.Email},</h2>
                <p>Click the link below to reset your password:</p>
                <a href='{resetLink}' style='color:#007bff;'>Reset Password</a>
                <p>This link will expire soon. If you didn't request a reset, please ignore this email.</p>",
                null
            );

            await _emailSender.SendEmailAsync(message);

            _logger.LogInformation("Password reset email sent successfully for user {UserId}", user.Id);

            return true;
        }
    }
}