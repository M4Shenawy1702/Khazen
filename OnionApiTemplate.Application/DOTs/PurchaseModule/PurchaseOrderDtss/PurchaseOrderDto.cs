using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss
{
    public class PurchaseOrderDto
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public PurchaseOrderStatus Status { get; set; }
        public Guid SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public decimal TotalAmount { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}
