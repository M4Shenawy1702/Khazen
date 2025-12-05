using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;
using Khazen.Application.DOTs.SalesModule.SalesOrderItemDtos;
using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.SalesModule.SalesOrderDtos
{
    public class SalesOrderDetailsDto
    {

        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }

        // Financials
        public decimal SubTotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }

        // Shipping
        public DateTime? EstimatedShipDate { get; set; }
        public DateTime? ActualShipDate { get; set; }
        public string? TrackingNumber { get; set; }

        // Customer
        public Guid CustomerId { get; set; }
        public string CustomerNameSnapshot { get; set; } = string.Empty;

        // Items
        public List<SalesOrderItemDto> Items { get; set; } = new();

        // Invoices
        public List<SalesInvoiceDto> Invoices { get; set; } = new();
    }
}

