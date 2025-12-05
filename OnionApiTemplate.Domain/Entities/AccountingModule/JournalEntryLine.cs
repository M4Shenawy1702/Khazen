namespace Khazen.Domain.Entities.AccountingModule
{
    public class JournalEntryLine : BaseEntity<int>
    {
        public Guid JournalEntryId { get; set; }
        public JournalEntry? JournalEntry { get; set; }

        public Guid AccountId { get; set; }
        public Account? Account { get; set; }

        public string? Description { get; set; }

        public decimal Debit { get; set; } = 0;
        public decimal Credit { get; set; } = 0;
    }
}
