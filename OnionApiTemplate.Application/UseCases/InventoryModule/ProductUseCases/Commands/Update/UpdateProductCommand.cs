using Khazen.Application.DOTs.InventoryModule.ProductDtos;

namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Update
{
    public record UpdateProductCommand(Guid Id, UpdateProductDto Dto) : IRequest<ProductDto>;
}
