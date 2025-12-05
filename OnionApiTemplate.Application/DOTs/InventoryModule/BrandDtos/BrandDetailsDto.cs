using Khazen.Application.DOTs.InventoryModule.ProductDtos;

namespace Khazen.Application.DOTs.InventoryModule.BrandDtos
{
    public class BrandDetailsDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Name { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string WebsiteUrl { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;

        public ICollection<ProductSnapshotDto> Products { get; set; } = [];
    }
}
