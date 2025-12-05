using Khazen.Application.DOTs.SalesModule.SalesOrderItemDtos;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Common.Interfaces.SalesModule.ISalesOrderModule
{
    internal interface IStockReservationService
    {
        Task ReserveStockAsync(IEnumerable<AddSalesOrderItemDto> items, IEnumerable<WarehouseProduct> warehouseProducts);
        Task ValidateReservedQuantitiesAsync(SalesOrder order, Dictionary<Guid, Product> productLookup);
        Task UnReserveStockAsync(SalesOrder salesOrder, IEnumerable<WarehouseProduct> warehouseProducts);
        Task UpdateReservedQuantitiesWhenOrderShipped(SalesOrder salesOrder, IEnumerable<WarehouseProduct> warehouseProducts);
        public void ReleaseOldReservations(SalesOrder oldOrder, IEnumerable<WarehouseProduct> warehouseProducts);
    }
}
