using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.BaseSpecifications.InventoryModule.BrandSpecifications
{
    public class GetAllBrandsSpecification : BaseSpecifications<Brand>
    {
        public GetAllBrandsSpecification()
            : base(b => true)
        {

        }
    }
}
