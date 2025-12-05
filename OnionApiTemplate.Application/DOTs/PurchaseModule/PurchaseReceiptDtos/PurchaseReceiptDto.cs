namespace Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos
{
    public class PurchaseReceiptDto
    {
        public Guid Id { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string? Notes { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public Guid ReceivedByEmployeeId { get; set; }
        public Guid WarehouseId { get; set; }
        public List<PurchaseReceiptItemDto> Items { get; set; } = [];
    }
}
