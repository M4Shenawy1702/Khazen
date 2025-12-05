using Khazen.Application.DOTs.InventoryModule.WarehouseDtos;

namespace Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Queries.GetById
{
    public record GetWarehouseByIdQuery(Guid Id) : IRequest<WarehouseDetailsDto>;
}
