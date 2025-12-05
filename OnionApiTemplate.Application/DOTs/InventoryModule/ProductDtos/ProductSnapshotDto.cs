namespace Khazen.Application.DOTs.InventoryModule.ProductDtos
{
    public class ProductSnapshotDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public decimal SellingPrice { get; set; }
    }
}
