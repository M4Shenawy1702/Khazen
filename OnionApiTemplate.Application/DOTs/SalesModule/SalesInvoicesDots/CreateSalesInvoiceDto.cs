using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.SalesModule.SalesInvoicesDots
{
    public class CreateSalesInvoiceDto
    {
        public Guid SalesOrderId { get; set; }

        public DateTime InvoiceDate { get; set; }
        public string? Notes { get; set; }

        public Guid CustomerId { get; set; }

        public DiscountType? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
    }
}
