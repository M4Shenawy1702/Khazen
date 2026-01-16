using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos
{
    public class PurchaseInvoiceDetailsDto
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? ModifiedBy { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }

        public Guid PurchaseReceiptId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        public InvoiceStatus InvoiceStatus { get; set; } = InvoiceStatus.Draft;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        public string? Notes { get; set; }

        public Guid JournalEntryId { get; set; }

        public Guid? ReversalJournalEntryId { get; set; }

        public ICollection<PurchaseInvoiceItemDto> Items { get; private set; } = [];
        public ICollection<PurchasePaymentDto> Payments { get; private set; } = [];

        public byte[]? RowVersion { get; private set; }
        public bool IsPosted => JournalEntryId != Guid.Empty;
        public bool IsReversed => ReversalJournalEntryId != null;
        public string? ReversedBy { get; set; }
        public DateTime? ReversedAt { get; set; }
    }
}
