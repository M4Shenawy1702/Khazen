using Microsoft.AspNetCore.Http;

namespace Khazen.Application.DOTs.InventoryModule.ProductDtos
{
    public class AddProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int MinimumStock { get; set; }
        public string SKU { get; set; }
        public Guid BrandId { get; set; }
        public Guid CategoryId { get; set; }
        public IFormFile Image { get; set; }
        public Guid WareHouseId { get; set; }
        public Guid SupplierId { get; set; }
    }
}
