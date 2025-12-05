namespace Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos
{
    public class UpdatePurchaseInvoiceDto
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string RowVersion { get; set; } = null!;
    }
}
