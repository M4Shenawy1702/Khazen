namespace Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos
{
    public class CreatePurchaseReceiptItemDto
    {
        public Guid ProductId { get; set; }
        public int ReceivedQuantity { get; set; }
    }
}
