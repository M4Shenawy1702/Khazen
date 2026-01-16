using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos
{
    public class PurchaseInvoiceDto
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        public InvoiceStatus InvoiceStatus { get; set; } = InvoiceStatus.Draft;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        public byte[]? RowVersion { get; private set; }
        public bool IsPosted { get; set; }
        public bool IsReversed { get; set; }
    }
}
