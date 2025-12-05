using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos
{
    public class SalesInvoicePaymentDto
    {
        public Guid Id { get; set; }
        public Guid SalesInvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod Method { get; set; }
        public bool IsDeleted { get; set; }
        public int JournalEntryId { get; set; }
        public int? ReversalJournalEntryId { get; set; }
    }
}
