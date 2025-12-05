using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss
{
    public class PurchaseOrderDto
    {
        public Guid Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Pending;

        public string SupplierName { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
    }
}
