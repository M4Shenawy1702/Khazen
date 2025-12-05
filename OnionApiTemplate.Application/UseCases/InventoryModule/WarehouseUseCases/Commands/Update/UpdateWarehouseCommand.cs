using Khazen.Application.DOTs.InventoryModule.WarehouseDtos;

namespace Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Update
{
    public record UpdateWarehouseCommand(Guid Id, UpdateWarehouseDto Dto, string ModifiedBy) : IRequest<WarehouseDto>;
}
