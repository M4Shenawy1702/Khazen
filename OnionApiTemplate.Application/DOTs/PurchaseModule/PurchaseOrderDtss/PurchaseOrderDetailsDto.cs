using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss
{
    public class PurchaseOrderDetailsDto
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public string? ModifiedBy { get; set; }
        public string OrderNumber { get; private set; } = string.Empty;
        public DateTime? DeliveryDate { get; private set; }

        public PurchaseOrderStatus Status { get; private set; } = PurchaseOrderStatus.Pending;
        public string? Notes { get; private set; }

        public Guid SupplierId { get; private set; }
        public Supplier? Supplier { get; private set; }

        public ICollection<PurchaseOrderItem> Items { get; private set; } = new List<PurchaseOrderItem>();
        public ICollection<PurchaseReceipt> PurchaseReceipts { get; private set; } = new List<PurchaseReceipt>();
        public ICollection<PurchaseInvoice> PurchaseInvoices { get; private set; } = new List<PurchaseInvoice>();
        public ICollection<PurchasePayment> PurchasePayments { get; private set; } = new List<PurchasePayment>();
        public decimal TotalAmount { get; set; }
    }
}
