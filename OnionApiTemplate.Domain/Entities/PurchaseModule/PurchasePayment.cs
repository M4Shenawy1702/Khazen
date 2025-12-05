using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Khazen.Domain.Entities.PurchaseModule
{
    public class PurchasePayment : BaseEntity<Guid>
    {
        public Guid PurchaseInvoiceId { get; private set; }
        public PurchaseInvoice PurchaseInvoice { get; private set; } = null!;

        public Guid? SafeTransactionId { get; private set; }
        public SafeTransaction? SafeTransaction { get; private set; }

        public Guid JournalEntryId { get; private set; }
        public JournalEntry? JournalEntry { get; private set; }

        public Guid? ReversalJournalEntryId { get; private set; }
        public JournalEntry? ReversalJournalEntry { get; private set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; private set; }

        public DateTime PaymentDate { get; private set; } = DateTime.UtcNow;
        public PaymentMethod Method { get; private set; } = PaymentMethod.Cash;

        public bool IsReversed { get; private set; }

        protected PurchasePayment() { }

        public PurchasePayment(decimal amount, PaymentMethod method, PurchaseInvoice invoice, SafeTransaction? safeTransaction = null)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));
            if (amount <= 0) throw new BadRequestException("Payment amount must be greater than zero.");
            if (invoice.IsReversed) throw new BadRequestException("Cannot pay a reversed invoice.");
            if (!invoice.IsPosted) throw new BadRequestException("Cannot pay an unposted invoice.");

            if (amount > invoice.RemainingAmount)
                throw new BadRequestException($"Payment exceeds invoice remaining amount ({invoice.RemainingAmount}).");

            Amount = amount;
            Method = method;
            PurchaseInvoice = invoice;
            PurchaseInvoiceId = invoice.Id;
            SafeTransaction = safeTransaction;
            SafeTransactionId = safeTransaction?.Id;

            invoice.AddPayment(this);
        }

        public void Reverse(Guid reversalJournalEntryId, string modifiedBy)
        {
            if (IsReversed) throw new BadRequestException("Payment already reversed.");
            if (!PurchaseInvoice.IsPosted) throw new BadRequestException("Cannot reverse payment on unposted invoice.");

            if (reversalJournalEntryId == Guid.Empty) throw new BadRequestException("ReversalJournalEntryId cannot be empty.");

            IsReversed = true;
            ReversalJournalEntryId = reversalJournalEntryId;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy;

            PurchaseInvoice.RecalculateTotals();
        }

        public void AssignJournalEntry(Guid journalEntryId)
        {
            if (journalEntryId == Guid.Empty) throw new BadRequestException("JournalEntryId cannot be empty.");
            JournalEntryId = journalEntryId;
        }
    }
}
