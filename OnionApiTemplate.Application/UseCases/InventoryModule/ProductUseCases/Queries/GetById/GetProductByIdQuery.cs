using Khazen.Application.DOTs.InventoryModule.ProductDtos;

namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Queries.GetById
{
    public record GetProductByIdQuery(Guid Id) : IRequest<ProductDetailsDto>;
}
