using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.Specification.InventoryModule.ProductSpecifications
{
    public class GetAllProductsByIdsSpecification : BaseSpecifications<Product>
    {
        public GetAllProductsByIdsSpecification(IList<Guid> productIds)
            : base(p => productIds.Contains(p.Id))
        {
        }
    }
}
