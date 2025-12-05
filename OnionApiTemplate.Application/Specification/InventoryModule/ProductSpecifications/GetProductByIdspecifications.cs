using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.BaseSpecifications.InventoryModule.ProductSpecifications
{
    public class GetProductByIdspecifications
        : BaseSpecifications<Product>
    {
        public GetProductByIdspecifications(Guid Id)
            : base(p => p.Id == Id)
        {
            AddInclude(p => p.Category!);
            AddInclude(p => p.Brand!);
            AddInclude(p => p.SupplierProducts!);
            AddInclude(p => p.WarehouseProducts!);
            AddInclude("SupplierProducts.Supplier");
            AddInclude("WarehouseProducts.Warehouse");

        }
    }
}
