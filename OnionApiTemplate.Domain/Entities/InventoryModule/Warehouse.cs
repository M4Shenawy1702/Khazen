using System.ComponentModel.DataAnnotations;

namespace Khazen.Domain.Entities.InventoryModule
{
    public class Warehouse : BaseEntity<Guid>
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [Phone, MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public ICollection<WarehouseProduct> WarehouseProducts { get; set; } = [];
        public string CreatedBy { get; set; } = string.Empty;
        public string? ModifiedBy { get; set; }
        public void Toggle(string modifiedBy)
        {
            IsDeleted = !IsDeleted;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
        }
    }
}
