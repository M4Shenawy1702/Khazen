using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.InventoryModule.ProductDtos;

namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Queries.GetAll
{
    public record GetAllProductsQuery(ProductsQueryParameters queryParameters) : IRequest<PaginatedResult<ProductDto>>;
}
