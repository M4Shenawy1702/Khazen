namespace Khazen.Domain.Entities.UsersModule
{
    public class RefreshToken : BaseEntity<Guid>
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresOnUtc { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOnUtc;

        public DateTime? RevokedAt { get; set; }
        public bool IsActive => RevokedAt == null && !IsExpired;

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
