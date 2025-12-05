namespace Khazen.Application.DOTs.InventoryModule.ProductDtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;

        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal AvgPrice { get; set; }

        public int QuantityInStock { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity { get; set; }

        public string SKU { get; set; } = string.Empty;

        public string BrandName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

    }
}
