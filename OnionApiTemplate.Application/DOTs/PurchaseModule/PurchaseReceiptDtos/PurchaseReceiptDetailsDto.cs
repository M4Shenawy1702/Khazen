using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.PurchaseModule;
using System.ComponentModel.DataAnnotations;

namespace Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos
{
    public class PurchaseReceiptDetailsDto
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? ModifiedBy { get; set; }
        public string ReceiptNumber { get; private set; } = default!;
        public DateTime ReceiptDate { get; private set; } = DateTime.UtcNow;
        public string? Notes { get; private set; }

        public Guid PurchaseOrderId { get; private set; }
        public PurchaseOrder? PurchaseOrder { get; private set; }

        public Guid WarehouseId { get; private set; }
        public Warehouse? Warehouse { get; private set; }

        public ICollection<PurchaseReceiptItem> Items { get; private set; } = new List<PurchaseReceiptItem>();

        public Guid? InvoiceId { get; private set; }
        public PurchaseInvoice? Invoice { get; private set; }

        public DateTime? DeletedAt { get; private set; }
        public string? DeletedBy { get; private set; }
        [Timestamp]
        public byte[]? RowVersion { get; private set; }
    }
}
