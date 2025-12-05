namespace Khazen.Application.DOTs.AccountingModule.JournalEntryDots
{
    public class JournalEntryDetailsDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string JournalEntryNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; }
        public string TransactionType { get; set; } = string.Empty;

        public Guid SafeTransactionId { get; set; }
        public Guid? SalesInvoiceId { get; set; }
        public Guid? PurchaseReceiptInvoiceId { get; set; }

        public List<JournalEntryLineDto> Lines { get; set; } = [];
    }
}
