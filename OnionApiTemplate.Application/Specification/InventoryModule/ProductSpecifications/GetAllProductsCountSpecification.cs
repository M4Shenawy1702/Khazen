using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.BaseSpecifications.InventoryModule.ProductSpecifications
{
    public class GetAllProductsCountSpecification
        : BaseSpecifications<Product>
    {
        public GetAllProductsCountSpecification(ProductsQueryParameters queryParameters)
            : base(d =>
            (queryParameters.ProductId == null || d.Id == queryParameters.ProductId) &&
            (string.IsNullOrWhiteSpace(queryParameters.ProductName) ||
             d.Name.ToLower().Trim().Contains(queryParameters.ProductName.ToLower().Trim())) &&
            (string.IsNullOrWhiteSpace(queryParameters.ProductSKU) ||
                 d.Name.ToLower().Trim().Contains(queryParameters.ProductSKU.ToLower().Trim()))
            )
        {
        }
    }
}
