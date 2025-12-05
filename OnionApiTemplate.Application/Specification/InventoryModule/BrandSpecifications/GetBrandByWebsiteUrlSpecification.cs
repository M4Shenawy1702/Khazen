using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.Specification.InventoryModule.BrandSpecifications
{
    internal class GetBrandByWebsiteUrlSpecification
        : BaseSpecifications<Brand>
    {
        public GetBrandByWebsiteUrlSpecification(string websiteUrl)
        : base(B => B.WebsiteUrl == websiteUrl)
        {
        }
    }
}
