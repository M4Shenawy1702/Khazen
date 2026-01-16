using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDots;
using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;
using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss
{
    public class PurchaseOrderDetailsDto
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public PurchaseOrderStatus Status { get; set; }
        public string? Notes { get; set; }

        public Guid SupplierId { get; set; }
        public string? SupplierName { get; set; }

        public ICollection<PurchaseOrderItemDto> Items { get; set; } = [];
        public ICollection<PurchaseReceiptDto> PurchaseReceipts { get; set; } = [];
        public ICollection<PurchaseInvoiceDto> PurchaseInvoices { get; set; } = [];
        public ICollection<PurchasePaymentDto> PurchasePayments { get; set; } = [];

        public decimal TotalAmount { get; set; }
        public byte[]? RowVersion { get; set; }
    }
}
