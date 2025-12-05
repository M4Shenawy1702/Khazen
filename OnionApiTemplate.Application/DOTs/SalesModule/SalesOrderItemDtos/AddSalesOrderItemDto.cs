using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.SalesModule.SalesOrderItemDtos
{
    public class AddSalesOrderItemDto
    {
        public Guid ProductId { get; set; }

        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }

        public int Quantity { get; set; }
    }
}
