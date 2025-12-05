namespace Khazen.Application.DOTs.PurchaseModule.SupplierDtos
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;

            //public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = [];
            //public ICollection<SupplierProduct> SupplierProducts { get; set; } = [];
    }
}