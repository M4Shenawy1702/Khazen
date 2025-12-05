using Khazen.Domain.Common.Enums;
using Khazen.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khazen.Domain.Entities.PurchaseModule
{
    public class PurchaseOrder : BaseEntity<Guid>
    {

        public string OrderNumber { get; private set; } = string.Empty;
        public DateTime? DeliveryDate { get; private set; }

        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Pending;
        public string? Notes { get; private set; }

        public Guid SupplierId { get; private set; }
        public Supplier? Supplier { get; private set; }

        public ICollection<PurchaseOrderItem> Items { get; private set; } = new List<PurchaseOrderItem>();
        public ICollection<PurchaseReceipt> PurchaseReceipts { get; private set; } = new List<PurchaseReceipt>();
        public ICollection<PurchaseInvoice> PurchaseInvoices { get; private set; } = new List<PurchaseInvoice>();
        public ICollection<PurchasePayment> PurchasePayments { get; private set; } = new List<PurchasePayment>();

        [NotMapped]
        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);

        [Timestamp]
        public byte[]? RowVersion { get; private set; }

        protected PurchaseOrder() { }

        public PurchaseOrder(Guid supplierId, string orderNumber, DateTime deliveryDate, string createdBy, string? notes = null)
        {
            SupplierId = supplierId;
            OrderNumber = orderNumber;
            DeliveryDate = deliveryDate;
            CreatedBy = createdBy;
            Notes = notes;
            CreatedAt = DateTime.UtcNow;
            Status = PurchaseOrderStatus.Pending;
        }
        public void Modify(Guid supplierId, string orderNumber, DateTime deliveryDate, string modifiedBy, string? notes = null)
        {
            SupplierId = supplierId;
            OrderNumber = orderNumber;
            DeliveryDate = deliveryDate;
            ModifiedBy = modifiedBy;
            Notes = notes;
            ModifiedAt = DateTime.UtcNow;
        }

        public void AddItem(PurchaseOrderItem item)
        {
            if (Status == PurchaseOrderStatus.Cancelled || Status == PurchaseOrderStatus.Paid)
                throw new InvalidOperationException("Cannot modify completed or cancelled PO.");

            Items.Add(item);
        }

        public void MarkApproved()
        {
            if (Status != PurchaseOrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be approved.");

            Status = PurchaseOrderStatus.Approved;
        }

        public void MarkPartiallyReceived()
        {
            Status = PurchaseOrderStatus.PartiallyReceived;
        }

        public void MarkFullyReceived()
        {
            Status = PurchaseOrderStatus.FullyReceived;
        }

        public void MarkInvoiced()
        {
            Status = PurchaseOrderStatus.Invoiced;
        }

        public void MarkPaid()
        {
            Status = PurchaseOrderStatus.Paid;
        }

        public void Cancel()
        {
            if (Status == PurchaseOrderStatus.Paid)
                throw new InvalidOperationException("Paid orders cannot be cancelled.");

            Status = PurchaseOrderStatus.Cancelled;
        }
        public void Toggle(string modifiedBy)
        {
            if (Status == PurchaseOrderStatus.Pending)
                Status = PurchaseOrderStatus.Cancelled;
            else if (Status == PurchaseOrderStatus.Cancelled)
                Status = PurchaseOrderStatus.Pending;
            else
                throw new DomainException("Only Pending or Cancelled orders can be toggled.");

            this.ModifiedBy = modifiedBy;
            this.ModifiedAt = DateTime.UtcNow;
        }
        public void SetRowVersion(byte[] rowVersion) => RowVersion = rowVersion;
        public void UpdateStatusBasedOnReceipts()
        {
            if (Items == null || Items.Count == 0)
            {
                Status = PurchaseOrderStatus.Pending;
                return;
            }

            bool anyReceived = false;
            bool allFullyReceived = true;

            foreach (var orderItem in Items)
            {
                var totalReceived = PurchaseReceipts
                    .Where(r => !r.IsDeleted)
                    .SelectMany(r => r.Items)
                    .Where(i => i.ProductId == orderItem.ProductId)
                    .Sum(i => i.ReceivedQuantity);

                if (totalReceived > 0)
                    anyReceived = true;

                if (totalReceived < orderItem.Quantity)
                    allFullyReceived = false;
            }

            Status = allFullyReceived switch
            {
                true => PurchaseOrderStatus.FullyReceived,
                false when anyReceived => PurchaseOrderStatus.PartiallyReceived,
                _ => PurchaseOrderStatus.Pending
            };
        }
    }
}
