namespace Khazen.Application.DOTs.SalesModule.SalesInvoicesDots
{
    public class SalesInvoiceDto
    {
        public Guid Id { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsVoided { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string PaymentStatus { get; private set; } = string.Empty;
    }
}
