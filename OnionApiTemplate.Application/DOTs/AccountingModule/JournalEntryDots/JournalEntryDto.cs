using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.DOTs.AccountingModule.JournalEntryDots
{
    public class JournalEntryDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string JournalEntryNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; } = DateTime.UtcNow;
        public TransactionType TransactionType { get; set; }

        public Guid? RelatedEntityId { get; set; }
        public RelatedEntityType? RelatedEntityType { get; set; }

        public Guid? ReversalOfJournalEntryId { get; set; }

        public List<JournalEntryLineDto> Lines { get; set; } = [];
    }
}
