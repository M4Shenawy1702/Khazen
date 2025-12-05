namespace Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots
{
    public class PurchasePaymentDto
    {
        public Guid Id { get; set; }
        public Guid PurchaseInvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Method { get; set; } = string.Empty;
        public bool IsReversed { get; private set; }
    }
}
