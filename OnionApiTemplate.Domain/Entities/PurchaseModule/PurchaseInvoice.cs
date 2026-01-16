using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.AccountingModule;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khazen.Domain.Entities.PurchaseModule
{
    public class PurchaseInvoice : BaseEntity<Guid>
    {
        public Guid SupplierId { get; private set; }
        public Supplier? Supplier { get; private set; }

        public Guid PurchaseReceiptId { get; private set; }
        public PurchaseReceipt? PurchaseReceipt { get; private set; }

        public string InvoiceNumber { get; private set; } = string.Empty;
        public DateTime InvoiceDate { get; private set; } = DateTime.UtcNow;

        public InvoiceStatus InvoiceStatus { get; private set; } = InvoiceStatus.Draft;
        public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Unpaid;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; private set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; private set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RemainingAmount { get; private set; }

        public string? Notes { get; private set; }

        public Guid JournalEntryId { get; set; }
        public JournalEntry? JournalEntry { get; private set; }

        public Guid? ReversalJournalEntryId { get; private set; }
        public JournalEntry? ReversalJournalEntry { get; private set; }

        public ICollection<PurchaseInvoiceItem> Items { get; private set; } = new List<PurchaseInvoiceItem>();
        public ICollection<PurchasePayment> Payments { get; private set; } = new List<PurchasePayment>();

        [Timestamp]
        public byte[]? RowVersion { get; private set; }
        public bool IsPosted => JournalEntryId != Guid.Empty;
        public bool IsReversed => ReversalJournalEntryId != null;
        public string? ReversedBy { get; set; }
        public DateTime? ReversedAt { get; set; }

        protected PurchaseInvoice() { }

        public PurchaseInvoice(Guid purchaseReceiptId, Guid supplierId, string invoiceNumber, string createdBy, DateTime? invoiceDate = null, string? notes = null)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber))
                throw new ArgumentException("Invoice number cannot be empty.");

            PurchaseReceiptId = purchaseReceiptId;
            SupplierId = supplierId;
            InvoiceNumber = invoiceNumber;
            InvoiceDate = invoiceDate ?? DateTime.UtcNow;
            Notes = notes;
            CreatedBy = createdBy;
            InvoiceStatus = InvoiceStatus.Draft;
            PaymentStatus = PaymentStatus.Unpaid;
            TotalAmount = 0m;
            PaidAmount = 0m;
            RemainingAmount = 0m;
        }
        public void Modify(string invoiceNumber, string modifiedBy, string? notes = null)
        {
            if (IsPosted || IsReversed)
                throw new InvalidOperationException("Cannot modify a posted or reversed invoice.");

            if (string.IsNullOrWhiteSpace(invoiceNumber))
                throw new ArgumentException("Invoice number cannot be empty.");
            ModifiedBy = modifiedBy;
            ModifiedAt = DateTime.UtcNow;
            InvoiceNumber = invoiceNumber;
            Notes = notes;
        }


        public void AddItem(PurchaseInvoiceItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            EnsureNotPostedOrReversed();

            Items.Add(item);
            RecalculateTotals();
        }

        public void RemoveItem(int itemId)
        {
            EnsureNotPostedOrReversed();
            var it = Items.FirstOrDefault(i => i.Id == itemId);
            if (it == null) return;
            Items.Remove(it);
            RecalculateTotals();
        }

        public void RecalculateTotals()
        {
            TotalAmount = Items?.Sum(i => i.TotalPrice) ?? 0m;

            PaidAmount = Payments?
                .Where(p => !p.IsReversed)
                .Sum(p => p.Amount) ?? 0m;

            RemainingAmount = Math.Round(TotalAmount - PaidAmount, 2);

            PaymentStatus = RemainingAmount <= 0m
                ? PaymentStatus.Paid
                : PaidAmount == 0m
                    ? PaymentStatus.Unpaid
                    : PaymentStatus.PartiallyPaid;

            ModifiedAt = DateTime.UtcNow;
        }

        public void AddPayment(PurchasePayment payment)
        {
            if (payment != null)
            {
                if (payment.Amount <= 0) throw new ArgumentException("Payment amount must be greater than zero.", nameof(payment));
                if (PaidAmount + payment.Amount > TotalAmount) throw new InvalidOperationException("Paid amount cannot exceed total invoice amount.");
                if (IsReversed) throw new InvalidOperationException("Cannot add payment to a reversed invoice.");

                Payments.Add(payment);
                RecalculateTotals();
            }
            else
                throw new ArgumentNullException(nameof(payment));
        }

        public void MarkAsPosted(Guid journalEntryId)
        {
            if (journalEntryId == Guid.Empty) throw new ArgumentNullException(nameof(journalEntryId));
            if (IsPosted) throw new InvalidOperationException("Invoice is already posted.");
            if (IsReversed) throw new InvalidOperationException("Cannot post a reversed invoice.");

            if (TotalAmount <= 0) throw new InvalidOperationException("Cannot post invoice with zero total.");

            JournalEntryId = journalEntryId;
            InvoiceStatus = InvoiceStatus.Posted;
        }

        public void Reverse(Guid reversalJournalEntryId, string reversedBy)
        {
            if (!IsPosted) throw new InvalidOperationException("Only posted invoices can be reversed.");
            if (IsReversed) throw new InvalidOperationException("Invoice already reversed.");
            if (reversalJournalEntryId == Guid.Empty) throw new ArgumentNullException(nameof(reversalJournalEntryId));

            ReversalJournalEntryId = reversalJournalEntryId;
            InvoiceStatus = InvoiceStatus.Reversed;
            IsDeleted = true;
            ReversedAt = DateTime.UtcNow;
            ReversedBy = reversedBy;
        }

        private void EnsureNotPostedOrReversed()
        {
            if (IsPosted) throw new InvalidOperationException("Cannot modify a posted invoice. Create reversal instead.");
            if (IsReversed) throw new InvalidOperationException("Cannot modify a reversed invoice.");
        }
    }
}