using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.DOTs.AccountingModule.JournalEntryDots
{
    public class JournalEntryDetailsDto
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }

        public string JournalEntryNumber { get; set; }
        public string Description { get; set; }
        public DateTime EntryDate { get; set; }
        public TransactionType TransactionType { get; set; }

        public Guid? RelatedEntityId { get; set; }
        public RelatedEntityType? RelatedEntityType { get; set; }

        public bool IsReversal { get; set; }
        public bool IsReversed { get; set; }
        public Guid? ReversalOfJournalEntryId { get; set; }
        public string? ReversedBy { get; set; }
        public DateTime? ReversedAt { get; set; }

        public List<JournalEntryLineDto> Lines { get; set; } = [];
    }
}
