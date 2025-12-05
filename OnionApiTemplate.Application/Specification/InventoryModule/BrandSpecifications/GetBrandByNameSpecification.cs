using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.BaseSpecifications.InventoryModule.BrandSpecifications
{
    internal class GetBrandByNameSpecification
        : BaseSpecifications<Brand>
    {
        public GetBrandByNameSpecification(string Name)
            : base(B => B.Name == Name)
        {
        }
    }
}
