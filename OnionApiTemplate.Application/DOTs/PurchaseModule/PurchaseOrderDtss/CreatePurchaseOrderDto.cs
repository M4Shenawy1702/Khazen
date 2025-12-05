namespace Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDots
{
    public class CreatePurchaseOrderDto
    {
        public Guid SupplierId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? Notes { get; set; }
        public string OrderNumber { get; set; } = null!;
        public List<CreatePurchaseOrderItemDto> Items { get; set; } = [];
    }
}
