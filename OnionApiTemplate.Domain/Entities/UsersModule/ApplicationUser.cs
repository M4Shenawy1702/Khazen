using Khazen.Domain.Entities.HRModule;
using Microsoft.AspNetCore.Identity;

namespace Khazen.Domain.Entities.UsersModule
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }

        public string? PhoneOtpCode { get; set; }
        public DateTime? PhoneOtpExpiry { get; set; }

        public Employee? Employee { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    }
    public enum Gender
    {
        Male,
        Female
    }
}
