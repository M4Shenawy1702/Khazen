namespace Khazen.Application.DOTs.Auth
{
    public record LoginRequestDto(string Email, string Password, string? RecaptchaToken);
}
