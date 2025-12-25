using Khazen.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Khazen.Domain.Entities.AccountingModule
{
    public class JournalEntry : BaseEntity<Guid>
    {
        private const decimal Tolerance = 0.0001m;

        public JournalEntry(string journalEntryNumber, string description, DateTime entryDate, TransactionType transactionType, RelatedEntityType? relatedEntityType, Guid? relatedEntityId, string createdBy = "System")
        {
            if (string.IsNullOrWhiteSpace(journalEntryNumber))
                throw new BadRequestException("Journal entry number cannot be empty.");

            if (string.IsNullOrWhiteSpace(description))
                throw new BadRequestException("Description cannot be empty.");

            if (entryDate > DateTime.UtcNow)
                throw new BadRequestException("Entry date cannot be in the future.");
            Id = Guid.NewGuid();
            JournalEntryNumber = journalEntryNumber;
            Description = description;
            EntryDate = entryDate;
            TransactionType = transactionType;
            RelatedEntityType = relatedEntityType.HasValue ? relatedEntityType : null;
            RelatedEntityId = relatedEntityId.HasValue ? relatedEntityId : null;
            CreatedBy = createdBy;
        }

        public string JournalEntryNumber { get; set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public DateTime EntryDate { get; private set; } = DateTime.UtcNow;
        public TransactionType TransactionType { get; private set; }

        public Guid? RelatedEntityId { get; set; }
        public RelatedEntityType? RelatedEntityType { get; set; }

        public bool IsReversal { get; private set; }
        public bool IsReversed { get; private set; }
        public Guid? ReversalOfJournalEntryId { get; private set; }
        public JournalEntry? ReversalOfJournalEntry { get; private set; }
        public string? ReversedBy { get; private set; }
        public DateTime? ReversedAt { get; private set; }

        [Timestamp]
        public byte[] RowVersion { get; private set; }

        public List<JournalEntryLine> Lines { get; private set; } = [];

        public void SetRowVersion(byte[] rowVersion) => RowVersion = rowVersion;
        public void Modify(string journalEntryNumber, string description, DateTime entryDate, TransactionType transactionType, string modifiedBy)
        {
            if (IsReversed)
                throw new BadRequestException("Cannot modify a reversed journal entry.");

            if (string.IsNullOrWhiteSpace(journalEntryNumber))
                throw new BadRequestException("Journal entry number cannot be empty.");

            JournalEntryNumber = journalEntryNumber;
            Description = description;
            EntryDate = entryDate;
            TransactionType = transactionType;

            ModifiedBy = modifiedBy;
            ModifiedAt = DateTime.UtcNow;

            ValidateBalance();
        }

        public void ValidateBalance()
        {
            var totalDebit = Lines.Sum(l => l.Debit);
            var totalCredit = Lines.Sum(l => l.Credit);

            if (Math.Abs(totalDebit - totalCredit) > Tolerance)
                throw new BadRequestException("Journal entry is not balanced. Debit must equal Credit.");
        }

        public JournalEntry CreateReversal(string reversedBy, string description)
        {
            if (IsReversed)
                throw new BadRequestException("This journal entry is already reversed.");

            var reversalTransactionType = TransactionType switch
            {
                TransactionType.SalesInvoice => TransactionType.SalesInvoiceReversal,
                TransactionType.PurchaseInvoice => TransactionType.PurchaseInvoiceReversal,
                TransactionType.SalesPayment => TransactionType.SalesPaymentReversal,
                TransactionType.PurchasePayment => TransactionType.PurchaseInvoiceReversal,
                TransactionType.SalaryExpense => TransactionType.PayrollReversal,
                _ => TransactionType
            };

            var reversal = new JournalEntry(
                journalEntryNumber: $"{JournalEntryNumber}-REV",
                description: description,
                entryDate: DateTime.UtcNow,
                transactionType: reversalTransactionType,
                relatedEntityType: RelatedEntityType,
                relatedEntityId: RelatedEntityId,
                createdBy: reversedBy
            );

            reversal.IsReversal = true;
            reversal.ReversalOfJournalEntryId = Id;

            reversal.Lines = Lines
                .Select(line => line.CreateReversal())
                .ToList();

            reversal.ValidateBalance();

            MarkAsReversed(reversedBy);

            return reversal;
        }


        private void MarkAsReversed(string reversedBy)
        {
            IsReversed = true;
            ReversedBy = reversedBy;
            ReversedAt = DateTime.UtcNow;
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
        PayrollReversal,
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
