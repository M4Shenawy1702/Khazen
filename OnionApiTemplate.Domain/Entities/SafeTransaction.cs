using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Domain.Entities
{
    public class SafeTransaction : BaseEntity<Guid>
    {
        public Guid SafeId { get; set; }
        public Safe Safe { get; set; } = null!;

        public DateTime Date { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public SafeTransactionType Type { get; set; }

        public Guid? JournalEntryId { get; set; }
        public JournalEntry? JournalEntry { get; set; }

        public string? Note { get; set; }

        public Guid? SourceId { get; set; }
        public TransactionSourceType SourceType { get; set; }
    }
    public enum SafeTransactionType
    {
        SalesPayment,
        PurchasePayment,
        Transfer,
        SalaryExpense,
        SalesPaymentReversal,
        PurchasePaymentReversal,
    }
    public enum TransactionSourceType
    {
        SalesInvoicePayment,
        PurchaseInvoicePayment,
        Salary,
        Transfer
    }
}
