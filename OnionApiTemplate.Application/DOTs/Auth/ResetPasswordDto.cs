namespace Khazen.Application.DOTs.Auth
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
