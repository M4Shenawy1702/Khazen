using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.BaseSpecifications.InventoryModule.ProductSpecifications
{
    public class GetSupplierProductsSpecifications
        : BaseSpecifications<Supplier>
    {
        public GetSupplierProductsSpecifications(IList<Guid> Ids)
            : base(s => Ids.Contains(s.Id))
        {

        }
    }
}
