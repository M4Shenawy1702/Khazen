namespace Khazen.Application.DOTs.InventoryModule.ProductDtos
{
    public class ProductDetailsDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;

        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal AvgPrice { get; set; }

        public int QuantityInStock { get; set; }
        public int ReservedQuantity { get; set; }

        public int MinimumStock { get; set; }
        public string SKU { get; set; } = string.Empty;

        public string BrandName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public ICollection<string> WarehouseNames { get; set; } = [];

        public ICollection<string> SupplierNames { get; set; } = [];
    }
}
