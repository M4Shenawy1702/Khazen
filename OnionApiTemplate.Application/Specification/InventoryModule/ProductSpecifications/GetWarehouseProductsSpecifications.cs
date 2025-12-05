using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.Specification.InventoryModule.ProductSpecifications
{
    public class GetWarehouseProductsSpecifications
        : BaseSpecifications<Warehouse>
    {
        public GetWarehouseProductsSpecifications(IList<Guid> Ids)
            : base(w => Ids.Contains(w.Id))
        {
        }
    }
}
