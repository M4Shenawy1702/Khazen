using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.BaseSpecifications.InventoryModule.ProductSpecifications
{
    internal class GetProductByNameSpecifications
        : BaseSpecifications<Product>
    {
        public GetProductByNameSpecifications(string Name)
            : base(p => p.Name == Name)
        {
        }
    }
}
