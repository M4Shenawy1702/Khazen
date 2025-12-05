using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;

namespace Khazen.Domain.Entities.SalesModule
{
    public class SalesInvoice : BaseEntity<Guid>
    {
        public Guid SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; } = null!;

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public Guid JournalEntryId { get; set; }
        public JournalEntry JournalEntry { get; set; } = null!;

        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public string Notes { get; set; } = string.Empty;

        public decimal SubTotal { get; set; } = 0m;
        public decimal DiscountAmount { get; set; } = 0m;
        public decimal TaxAmount { get; set; } = 0m;
        public decimal GrandTotal { get; set; } = 0m;

        public string? PostedBy { get; private set; }
        public bool IsPosted { get; private set; }
        public DateTime? PostedAt { get; set; }

        public InvoiceStatus InvoiceStatus { get; private set; } = InvoiceStatus.Draft;

        public ICollection<SalesInvoicePayment> Payments { get; set; } = new List<SalesInvoicePayment>();
        public ICollection<SalesInvoiceItem> Items { get; set; } = new List<SalesInvoiceItem>();

        public decimal TotalPaid => Payments.Where(p => !p.IsReversed).Sum(p => p.Amount);
        public decimal RemainingAmount => Math.Max(GrandTotal - TotalPaid, 0m);

        public string? VoidedBy { get; private set; }
        public bool IsVoided { get; private set; }
        public DateTime? VoidedAt { get; private set; }

        public void Modify(DateTime invoiceDate, string? notes, Guid customerId, string modifiedBy)
        {
            InvoiceDate = invoiceDate;
            Notes = notes ?? this.Notes;
            CustomerId = customerId;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
        }
        public void UpdateInvoiceStatus()
        {
            if (!IsPosted)
            {
                InvoiceStatus = InvoiceStatus.Draft;
                return;
            }

            if (TotalPaid == 0)
                InvoiceStatus = InvoiceStatus.Posted;
            else if (TotalPaid < GrandTotal)
                InvoiceStatus = InvoiceStatus.PartiallyPaid;
            else
                InvoiceStatus = InvoiceStatus.Paid;
        }

        public void MarkAsPosted(string postedBy)
        {
            if (IsVoided) throw new BadRequestException("Cannot post a voided invoice.");
            if (IsPosted) throw new BadRequestException("Invoice is already posted.");
            PostedBy = postedBy;
            PostedAt = DateTime.UtcNow;

            UpdateInvoiceStatus();
        }
        public void Void(string voidedBy)
        {
            if (IsVoided) throw new BadRequestException("Invoice already voided.");
            IsVoided = true;
            VoidedBy = voidedBy;
            VoidedAt = DateTime.UtcNow;
        }

        public void MarkAsNotVoid()
        {
            if (!IsVoided) throw new BadRequestException("Invoice is not voided.");
            IsVoided = false;
            VoidedAt = null;
        }

        public void CalculateTotals()
        {
            SubTotal = Items.Sum(i => i.Quantity * i.UnitPrice);
            DiscountAmount = Items.Sum(i => i.DiscountValue);
            TaxAmount = Items.Sum(i => i.TaxAmount);
            GrandTotal = SubTotal - DiscountAmount + TaxAmount;

            UpdateInvoiceStatus();
        }
    }
}
