namespace Khazen.Application.DOTs.SalesModule.SalesInvoicesDots
{
    public class UpdateSalesInvoiceDto
    {
        public Guid CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Notes { get; set; }
    }
}
