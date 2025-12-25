namespace Khazen.Application.DOTs.AccountingModule.JournalEntryDots;

public class JournalEntryLineDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Guid JournalEntryId { get; private set; }

    public Guid AccountId { get; private set; }

    public string? Description { get; private set; }

    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }
}
