using Microsoft.AspNetCore.Identity;

namespace Khazen.Domain.Entities.UsersModule
{
    public class ApplicationRole : IdentityRole
    {
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public string? ToggeledBy { get; set; }
        public DateTime? ToggeledAt { get; set; }
        public string? RevokedBy { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
