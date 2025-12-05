using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots
{
    public class CreatePurchasePaymentDto
    {
        public Guid PurchaseInvoiceId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
    }
}
