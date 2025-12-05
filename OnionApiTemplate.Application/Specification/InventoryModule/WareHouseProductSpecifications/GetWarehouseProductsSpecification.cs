using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.InventoryModule;
namespace Khazen.Application.Specification.InventoryModule.WareHouseProductSpecifications
{
    public class GetWarehouseProductsSpecification
        : BaseSpecifications<WarehouseProduct>
    {
        public GetWarehouseProductsSpecification(List<Guid> productIds)
            : base(wp => productIds.Contains(wp.ProductId))
        {
            AddInclude(wp => wp.Product);
        }
    }

}
