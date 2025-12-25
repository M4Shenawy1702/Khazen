namespace Khazen.Application.DOTs.Auth
{
    public record ForgetPasswordDto(string Email, string ClientUrl, string? RecaptchaToken);
}
