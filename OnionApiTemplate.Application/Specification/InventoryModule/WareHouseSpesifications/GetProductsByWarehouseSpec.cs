using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.Specification.InventoryModule.WareHouseSpesifications
{
    public class GetProductsByWarehouseSpec : BaseSpecifications<WarehouseProduct>
    {
        public GetProductsByWarehouseSpec(Guid warehouseId, List<Guid> productIds)
            : base(x => x.WarehouseId == warehouseId && productIds.Contains(x.ProductId))
        {
        }
    }
}
