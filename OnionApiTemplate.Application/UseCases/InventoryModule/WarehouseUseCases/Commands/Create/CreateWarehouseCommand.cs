using Khazen.Application.DOTs.InventoryModule.WarehouseDtos;

namespace Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Create
{
    public record CreateWarehouseCommand(CreateWarehouseDto Dto, string CurrentUserId) : IRequest<WarehouseDto>;
}
