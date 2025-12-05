namespace Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDots
{
    public class PurchaseOrderItemDto
    {
        public Guid PurchaseOrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ExpectedUnitPrice { get; set; }
        public decimal TotalPrice => Quantity * ExpectedUnitPrice;
    }
}
