using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.SalesModule.SalesOrderDtos
{
    public class SalesOrderDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal RemainingAmount { get; set; }
        public string CustomerNameSnapshot { get; set; } = string.Empty;
    }
}
