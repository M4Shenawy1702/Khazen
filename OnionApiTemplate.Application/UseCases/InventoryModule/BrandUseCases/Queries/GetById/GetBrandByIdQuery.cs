using Khazen.Application.DOTs.InventoryModule.BrandDtos;

namespace Khazen.Application.UseCases.InventoryModule.BrandUseCases.Queries.GetById
{
    public record GetBrandByIdQuery(Guid Id) : IRequest<BrandDetailsDto>;
}
