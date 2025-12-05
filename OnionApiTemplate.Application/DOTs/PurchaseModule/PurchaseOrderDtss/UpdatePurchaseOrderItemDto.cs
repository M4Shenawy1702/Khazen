namespace Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDots
{
    public class UpdatePurchaseOrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ExpectedUnitPrice { get; set; }
    }

}
