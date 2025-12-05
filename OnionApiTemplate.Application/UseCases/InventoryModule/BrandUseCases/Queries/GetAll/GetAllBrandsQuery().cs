using Khazen.Application.DOTs.InventoryModule.BrandDtos;

namespace Khazen.Application.UseCases.InventoryModule.BrandUseCases.Queries.GetAll
{
    public record GetAllBrandsQuery() : IRequest<IEnumerable<BrandDto>>;
}
