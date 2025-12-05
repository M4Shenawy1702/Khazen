using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Khazen.Domain.Entities.SalesModule
{
    public class SalesInvoicePayment : BaseEntity<Guid>
    {
        public Guid SalesInvoiceId { get; set; }
        public SalesInvoice SalesInvoice { get; set; } = null!;

        public decimal Amount { get; set; }
        public string? Notes { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod Method { get; set; } = PaymentMethod.Cash;

        public ICollection<SafeTransaction> SafeTransactions = [];

        public Guid JournalEntryId { get; set; }
        public JournalEntry JournalEntry { get; set; } = null!;

        public Guid? ReversalJournalEntryId { get; set; }
        public JournalEntry? ReversalJournalEntry { get; set; }

        public bool IsReversed { get; private set; }

        [Timestamp]
        public byte[] RowVersion { get; private set; }
        public void AssertRowVersion(byte[]? requestVersion)
        {
            if (requestVersion is null)
                throw new ConcurrencyException("RowVersion is missing.");

            if (RowVersion is null)
                throw new ConcurrencyException("Entity RowVersion is missing.");

            if (!RowVersion.SequenceEqual(requestVersion))
                throw new ConcurrencyException("Order was modified by another user.");
        }
        public void Reverse()
        {
            if (IsReversed) throw new BadRequestException("Payment already reversed.");
            IsReversed = true;
        }
    }
}
