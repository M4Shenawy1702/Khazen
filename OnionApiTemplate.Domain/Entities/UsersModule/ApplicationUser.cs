using Khazen.Domain.Entities.HRModule;
using Khazen.Domain.Entities.SalesModule;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Khazen.Domain.Entities.UsersModule
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public Gender Gender { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public string? PhoneOtpCode { get; set; }
        public DateTime? PhoneOtpExpiry { get; set; }
        public int? PhoneOtpFailedAttempts { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Employee? Employee { get; set; }
        public Customer? Customer { get; set; }
        public UserType UserType { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

        public void SetRowVersion(byte[] rowVersion) => RowVersion = rowVersion;
    }

    public enum Gender
    {
        Male,
        Female
    }
    public enum UserType
    {
        Employee,
        Customer
    }
}