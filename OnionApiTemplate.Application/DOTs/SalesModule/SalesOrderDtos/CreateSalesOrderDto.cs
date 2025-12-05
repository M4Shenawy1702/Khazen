using Khazen.Application.DOTs.SalesModule.SalesOrderItemDtos;
using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.SalesModule.SalesOrderDtos
{
    public record CreateSalesOrderDto(DateTime? EstimatedShipDate, DiscountType DiscountType, decimal DiscountValue, Guid CustomerId, IEnumerable<AddSalesOrderItemDto> SalesOrderItems);

}
