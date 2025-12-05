using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.Specification.InventoryModule.WareHouseSpesifications
{
    public class GetProductWareHouseSpec
        : BaseSpecifications<WarehouseProduct>
    {
        public GetProductWareHouseSpec(Guid warehouseId, Guid productId) :
            base(wp => wp.WarehouseId == warehouseId && wp.ProductId == productId)
        {
        }
    }
}
