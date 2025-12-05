using Khazen.Application.DOTs.InventoryModule.BrandDtos;

namespace Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Create
{
    public record CreateBrandCommand(CreateBrandDto Dto, string CreatedBy) : IRequest<BrandDto>;
}
