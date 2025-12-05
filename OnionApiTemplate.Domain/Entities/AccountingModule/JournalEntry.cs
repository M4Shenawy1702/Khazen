using Khazen.Domain.Exceptions;

namespace Khazen.Domain.Entities.AccountingModule
{
    public class JournalEntry : BaseEntity<Guid>
    {
        public string JournalEntryNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; } = DateTime.UtcNow;
        public TransactionType TransactionType { get; set; }

        public Guid? RelatedEntityId { get; set; }
        public RelatedEntityType? RelatedEntityType { get; set; }

        public Guid? ReversalOfJournalEntryId { get; set; }
        public JournalEntry? ReversalOfJournalEntry { get; set; }

        public bool IsReversal { get; set; }
        public bool IsReversed { get; set; }
        public DateTime? ReversedAt { get; set; }


        public List<JournalEntryLine> Lines { get; set; } = [];
        public void ValidateBalance()
        {
            var totalDebit = Lines.Sum(l => l.Debit);
            var totalCredit = Lines.Sum(l => l.Credit);

            if (totalDebit != totalCredit)
                throw new BadRequestException("Journal entry is not balanced. Debit must equal Credit.");
        }
    }

    public enum RelatedEntityType
    {
        SalesInvoice,
        PurchaseInvoice,
        SalesInvoicePayment,
        PurchaseInvoicePayment,
        Expense,
        SalaryExpense,
        Advance,
        Transfer
    }

    public enum TransactionType
    {
        CashSale,
        CashPurchase,
        Expense,
        SalaryExpense,
        Advance,
        Transfer,
        Refund,
        SalesInvoice,
        PurchaseInvoice,
        PurchaseReceipt,
        PurchasePayment,
        SalesPayment,
        SalesInvoiceReversal,
        PurchaseInvoiceReversal,
        SalesPaymentReversal
    }
}
