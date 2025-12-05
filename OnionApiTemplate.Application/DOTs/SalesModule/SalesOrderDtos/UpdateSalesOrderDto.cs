using Khazen.Application.DOTs.SalesModule.SalesOrderItemDtos;
using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.SalesModule.SalesOrderDtos
{
    public record UpdateSalesOrderDto(DateTime? EstimatedShipDate, DiscountType DiscountType, decimal DiscountValue, Guid CustomerId, ICollection<AddSalesOrderItemDto> SalesOrderItems);
}
