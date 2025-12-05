using Khazen.Application.DOTs.InventoryModule.ProductDtos;

namespace Khazen.Application.DOTs.InventoryModule.CategoryDots
{
    public class CategoryDetailsDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<ProductSnapshotDto> Products { get; set; } = [];
    }
}
