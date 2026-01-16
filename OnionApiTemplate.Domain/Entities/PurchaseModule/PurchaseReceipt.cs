using Khazen.Domain.Entities.InventoryModule;
using System.ComponentModel.DataAnnotations;

namespace Khazen.Domain.Entities.PurchaseModule
{
    public class PurchaseReceipt : BaseEntity<Guid>
    {
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

        protected PurchaseReceipt() { }

        public PurchaseReceipt(Guid purchaseOrderId, Guid warehouseId, string createdBy, string? notes = null)
        {
            PurchaseOrderId = purchaseOrderId;
            WarehouseId = warehouseId;
            Notes = notes;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            ReceiptDate = DateTime.UtcNow;
        }

        public void SetReceiptNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Receipt number is required.");

            ReceiptNumber = number;
        }

        public void SetRowVersion(byte[]? value)
        {
            RowVersion = value;
        }

        public void AddItem(PurchaseReceiptItem item)
        {
            if (item.ReceivedQuantity <= 0)
                throw new InvalidOperationException("Item quantity must be greater than zero.");

            Items.Add(item);
        }

        public void MarkAsDeleted(string deletedBy)
        {
            if (IsDeleted)
                throw new InvalidOperationException("Receipt is already deleted.");

            IsDeleted = true;

            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
        }

    }
}
