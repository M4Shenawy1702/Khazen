using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.BaseSpecifications.InventoryModule.ProductSpecifications
{
    public class GetProductBySKUSpecifications
        : BaseSpecifications<Product>
    {
        public GetProductBySKUSpecifications(string SKU)
            : base(p => p.SKU == SKU)
        {
        }
    }
}
