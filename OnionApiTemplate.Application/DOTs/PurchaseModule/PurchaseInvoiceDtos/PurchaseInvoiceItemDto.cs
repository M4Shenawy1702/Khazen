namespace Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos
{
    public class PurchaseInvoiceItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

}
