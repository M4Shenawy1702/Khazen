using Khazen.Application.DOTs.InventoryModule.ProductDtos;

namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Add
{
    public record AddProductCommand(AddProductDto Dto) : IRequest<ProductDto>;
}
