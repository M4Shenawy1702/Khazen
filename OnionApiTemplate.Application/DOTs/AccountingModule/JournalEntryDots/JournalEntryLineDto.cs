namespace Khazen.Application.DOTs.AccountingModule.JournalEntryDots;

public class JournalEntryLineDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int JournalEntryId { get; set; }

    public Guid AccountId { get; set; }
    public string? Description { get; set; }
    public decimal Debit { get; set; } = 0;
    public decimal Credit { get; set; } = 0;
}
