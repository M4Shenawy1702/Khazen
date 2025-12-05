namespace Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos
{
    public class CreatePurchaseInvoiceDto
    {
        public Guid PurchaseReceiptId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string? Notes { get; set; }
        public List<CreatePurchaseInvoiceItemDto> Items { get; set; } = [];
    }
}
