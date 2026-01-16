namespace Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos
{
    public class UpdatePurchaseReceiptDto
    {
        public DateTime? ReceiptDate { get; set; }
        public string? Notes { get; set; }
        public byte[] RowVersion { get; set; } = null!;
        public byte[] OrderRowVersion { get; set; } = null!;
        public List<UpdatePurchaseReceiptItemDto>? Items { get; set; }
    }
}
