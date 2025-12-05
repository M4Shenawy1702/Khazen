using Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos;
using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.SalesModule.SalesInvoicesDots
{
    public class SalesInvoiceDetailsDto
    {
        public Guid SalesOrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; } = string.Empty;

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }

        public decimal Discount { get; set; }
        public decimal Tax { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public ICollection<SalesInvoicePaymentDto> Payments { get; set; } = [];

        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }

        public bool IsVoided { get; set; }
        public DateTime? VoidedAt { get; set; }
    }
}
