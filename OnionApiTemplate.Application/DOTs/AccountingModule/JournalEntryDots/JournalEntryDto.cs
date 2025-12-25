using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.DOTs.AccountingModule.JournalEntryDots
{
    public class JournalEntryDto
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }

        public string JournalEntryNumber { get; set; }
        public DateTime EntryDate { get; set; }
        public TransactionType TransactionType { get; set; }

        public bool IsReversal { get; set; }
        public bool IsReversed { get; set; }
        public Guid? ReversalOfJournalEntryId { get; set; }

        public List<JournalEntryLineDto> Lines { get; set; } = [];
    }
}
