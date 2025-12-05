using System.ComponentModel.DataAnnotations;

namespace Khazen.Domain.Entities.InventoryModule
{
    public class Brand : BaseEntity<Guid>
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? LogoUrl { get; set; }

        [Url, MaxLength(200)]
        public string? WebsiteUrl { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? ContactEmail { get; set; }

        public ICollection<Product> Products { get; set; } = [];
        public void Toggle(string modifiedBy)
        {
            IsDeleted = !IsDeleted;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
        }
    }
}
