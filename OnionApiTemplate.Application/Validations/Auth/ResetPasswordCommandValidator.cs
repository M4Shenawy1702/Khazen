using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.ResetPasswordDto.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.ResetPasswordDto.Token).NotEmpty();
        RuleFor(x => x.ResetPasswordDto.NewPassword)
            .NotEmpty()
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
    }
}