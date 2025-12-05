using Khazen.Application.DOTs.InventoryModule.WarehouseDtos;

namespace Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Queries.GetAll
{
    public record GetAllWarehousesQuery() : IRequest<IEnumerable<WarehouseDto>>;
}
