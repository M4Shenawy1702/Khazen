using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.Specification.InventoryModule.WareHouseSpesifications
{
    public class GetWareHouseByIdSpec
        : BaseSpecifications<Warehouse>
    {
        public GetWareHouseByIdSpec(Guid id)
            : base(w => w.Id == id)
        {
            AddInclude(w => w.WarehouseProducts);
            AddInclude("WarehouseProducts.Product");
        }
    }
}
