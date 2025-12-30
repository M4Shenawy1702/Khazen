using Khazen.Application.DOTs.InventoryModule.BrandDtos;

namespace Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Update
{
    public record UpdateBrandCommand(Guid Id, UpdateBrandDto Dto, string CurrentUserId) : IRequest<BrandDetailsDto>;
}
