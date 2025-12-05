namespace Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos
{
    public class PurchaseReceiptItemDto
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal ReceivedQuantity { get; set; }
    }
}
