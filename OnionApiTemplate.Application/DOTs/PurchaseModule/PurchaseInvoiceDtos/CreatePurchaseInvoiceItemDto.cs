namespace Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos
{
    public class CreatePurchaseInvoiceItemDto
    {
        public Guid ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal AdditionalCharges { get; set; } = 0;
        public string Notes { get; set; } = string.Empty;
    }
}
