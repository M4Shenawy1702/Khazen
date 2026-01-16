namespace Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos
{
    public class PurchaseReceiptDto
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string? Notes { get; set; }

        public Guid PurchaseOrderId { get; set; }

        public Guid WarehouseId { get; set; }

        public Guid? InvoiceId { get; set; }

        public byte[]? RowVersion { get; set; }
    }
}
