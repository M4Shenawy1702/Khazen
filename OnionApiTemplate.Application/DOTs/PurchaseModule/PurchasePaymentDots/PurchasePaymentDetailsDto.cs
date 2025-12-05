using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots
{
    public class PurchasePaymentDetailsDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid PurchaseInvoiceId { get; set; }
        public PurchaseInvoice PurchaseInvoice { get; set; } = null!;

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod Method { get; set; } = PaymentMethod.Cash;

        public int JournalEntryId { get; set; }
        public JournalEntry? JournalEntry { get; set; }
    }
}
