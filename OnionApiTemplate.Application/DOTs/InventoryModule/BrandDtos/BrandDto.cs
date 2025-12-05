namespace Khazen.Application.DOTs.InventoryModule.BrandDtos
{
    public class BrandDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string WebsiteUrl { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
    }
}
