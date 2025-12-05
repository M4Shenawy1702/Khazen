using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.BaseSpecifications.InventoryModule.BrandSpecifications
{
    public class GetBrandByIdSpecification : BaseSpecifications<Brand>
    {
        public GetBrandByIdSpecification(Guid id)
            : base(b => b.Id == id)
        {
        }
    }
}