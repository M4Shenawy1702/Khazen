namespace Khazen.Application.DOTs.InventoryModule.BrandDtos
{
    public class CreateBrandDto
    {
        public string Name { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? ContactEmail { get; set; }
    }
}
