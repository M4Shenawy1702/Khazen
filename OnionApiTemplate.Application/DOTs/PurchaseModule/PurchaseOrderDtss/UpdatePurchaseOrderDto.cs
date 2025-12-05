namespace Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDots
{
    public class UpdatePurchaseOrderDto
    {
        public DateTime DeliveryDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = null!;
        public Guid SupplierId { get; set; } = Guid.Empty;
        public byte[] RowVersion { get; set; } = null!;
        public List<UpdatePurchaseOrderItemDto> Items { get; set; } = new();
    }

}
