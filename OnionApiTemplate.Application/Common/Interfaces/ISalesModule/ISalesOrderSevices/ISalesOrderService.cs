using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Common.Interfaces.SalesModule.ISalesOrderModule
{
    internal interface ISalesOrderService
    {
        SalesOrder CreateSalesOreder(Customer customer, CreateSalesOrderDto dto, IEnumerable<WarehouseProduct> warehouseProducts, string createdBy);
        Task<SalesOrder> UpdateSalesOrderAsync(Customer customer, UpdateSalesOrderDto dto, SalesOrder salesOrder, IEnumerable<WarehouseProduct> warehouseProducts, string modifiedBy);
    }
}
