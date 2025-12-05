using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots
{
    public class UpdatePurchasePaymentDto
    {
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
    }
}
