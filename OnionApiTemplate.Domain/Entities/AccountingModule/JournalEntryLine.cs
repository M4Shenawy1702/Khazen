using Khazen.Domain.Exceptions;

namespace Khazen.Domain.Entities.AccountingModule
{
    public class JournalEntryLine : BaseEntity<int>
    {
        public Guid JournalEntryId { get; private set; }
        public JournalEntry? JournalEntry { get; set; }

        public Guid AccountId { get; private set; }
        public Account Account { get; set; }

        public string? Description { get; private set; }

        public decimal Debit { get; private set; }
        public decimal Credit { get; private set; }

        public JournalEntryLine(Guid accountId, decimal debit, decimal credit, string? description = null)
        {
            if (accountId == Guid.Empty)
                throw new BadRequestException("AccountId cannot be empty.");

            if (debit < 0 || credit < 0)
                throw new BadRequestException("Debit and credit cannot be negative.");

            if (debit > 0 && credit > 0)
                throw new BadRequestException("A journal entry line cannot have both debit and credit.");

            if (debit == 0 && credit == 0)
                throw new BadRequestException("A journal entry line must have either debit or credit.");

            AccountId = accountId;
            Debit = debit;
            Credit = credit;
            Description = description;
        }

        public void AttachToJournalEntry(Guid journalEntryId)
        {
            if (journalEntryId == Guid.Empty)
                throw new BadRequestException("Invalid JournalEntryId.");

            JournalEntryId = journalEntryId;
        }

        public JournalEntryLine CreateReversal()
        {
            return new JournalEntryLine(
                accountId: AccountId,
                debit: Credit,
                credit: Debit,
                description: $"Reversal of line {Id}"
            );
        }
    }
}
