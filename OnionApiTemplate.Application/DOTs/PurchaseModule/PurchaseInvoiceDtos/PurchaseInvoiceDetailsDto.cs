using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos
{
    public class PurchaseInvoiceDetailsDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid SupplierId { get; set; }

        public Guid PurchaseReceiptId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public InvoiceStatus InvoiceStatus { get; set; } = InvoiceStatus.Draft;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        public decimal TotalAmount { get; private set; }
        public decimal PaidAmount { get; private set; }
        public decimal RemainingAmount { get; private set; }

        public string? Notes { get; set; } = string.Empty;

        public Guid JournalEntryId { get; set; }
        public Guid? ReversalJournalEntryId { get; set; }

        public ICollection<PurchaseInvoiceItemDto> Items { get; set; } = [];
        public ICollection<PurchasePaymentDto> Payments { get; set; } = [];
    }
}
