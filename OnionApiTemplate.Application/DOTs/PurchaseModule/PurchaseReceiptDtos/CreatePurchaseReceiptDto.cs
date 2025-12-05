namespace Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos
{
    public class CreatePurchaseReceiptDto
    {
        public DateTime ReceiptDate { get; set; }
        public string? Notes { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public Guid ReceivedByEmployeeId { get; set; }
        public Guid WarehouseId { get; set; }
        public byte[] RowVersion { get; set; } = null!;
        public List<CreatePurchaseReceiptItemDto> Items { get; set; } = [];
    }
}
