namespace Khazen.Domain.Entities.PurchaseModule
{
    public class Supplier : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = [];
        public ICollection<SupplierProduct> SupplierProducts { get; set; } = [];

        public void Toggle(string modifiedBy)
        {
            IsActive = !IsActive;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
        }
    }
}