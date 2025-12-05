using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.DOTs.AccountingModule.JournalEntryDots
{
    public class CreateJournalEntryDto
    {
        public string Description { get; set; } = null!;
        public DateTime EntryDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public List<CreateJournalEntryLineDto> Lines { get; set; } = [];
    }
}
