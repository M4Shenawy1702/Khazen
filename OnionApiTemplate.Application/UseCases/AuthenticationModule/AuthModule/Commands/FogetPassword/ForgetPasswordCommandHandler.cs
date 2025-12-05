using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.ObjectValues;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.FogetPassword;
using Khazen.Domain.Exceptions;
using MimeKit;

internal class ForgetPasswordCommandHandler(
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender,
    IValidator<ForgetPasswordCommand> validator)
    : IRequestHandler<ForgetPasswordCommand, bool>
{
    public async Task<bool> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            throw new BadRequestException(validation.Errors.Select(e => e.ErrorMessage).ToList());

        var user = await userManager.FindByEmailAsync(request.ForgetPasswordDto.Email)
                   ?? throw new NotFoundException<ApplicationUser>(request.ForgetPasswordDto.Email);

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var resetLink = $"{request.ForgetPasswordDto.ClientUrl}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email!)}";

        var message = new EmailMessage(
            new List<MailboxAddress> { new MailboxAddress(user.UserName ?? user.Email, user.Email!) },
            "Password Reset Request",
            $@"<h2>Hello {user.UserName ?? ""},</h2>
              <p>Click the link below to reset your password:</p>
              <a href='{resetLink}' style='color:#007bff;'>Reset Password</a>
              <p>This link will expire soon. If you didn't request a reset, please ignore this email.</p>",
            null
        );

        await emailSender.SendEmailAsync(message);
        return true;
    }
}
