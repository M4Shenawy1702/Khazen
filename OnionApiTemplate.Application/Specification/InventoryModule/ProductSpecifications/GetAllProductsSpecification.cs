using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.BaseSpecifications.InventoryModule.ProductSpecifications
{
    public class GetAllProductsSpecification
        : BaseSpecifications<Product>
    {
        public GetAllProductsSpecification(ProductsQueryParameters queryParameters)
    : base(d =>
        (queryParameters.ProductId == null || d.Id == queryParameters.ProductId) &&
        (string.IsNullOrWhiteSpace(queryParameters.ProductName) ||
         d.Name.ToLower().Trim().Contains(queryParameters.ProductName.ToLower().Trim())) &&
        (string.IsNullOrWhiteSpace(queryParameters.ProductSKU) ||
             d.Name.ToLower().Trim().Contains(queryParameters.ProductSKU.ToLower().Trim()))
        )
        {
            AddInclude(p => p.Category!);
            AddInclude(p => p.Brand!);
            AddInclude(p => p.SupplierProducts!);
            AddInclude(p => p.WarehouseProducts!);
            ApplyPagination(queryParameters.PageSize, queryParameters.PageIndex);
        }
        public GetAllProductsSpecification(List<Guid> Ids)
    : base(d => Ids.Contains(d.Id))
        {

        }
    }
}

