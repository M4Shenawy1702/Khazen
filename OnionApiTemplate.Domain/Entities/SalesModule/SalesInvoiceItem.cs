using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Domain.Entities.SalesModule
{
    public class SalesInvoiceItem : BaseEntity<int>
    {
        public Guid SalesInvoiceId { get; set; }
        public SalesInvoice SalesInvoice { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }

        public decimal SubTotal { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal LineTotal { get; private set; }

        public void Calculate(decimal taxRate)
        {
            SubTotal = Quantity * UnitPrice;

            DiscountAmount = DiscountType switch
            {
                Common.Enums.DiscountType.FixedAmount => DiscountValue,
                Common.Enums.DiscountType.Percentage => (SubTotal * (DiscountValue) / 100),
                _ => 0
            };

            var taxableAmount = SubTotal - DiscountAmount;
            TaxAmount = taxableAmount * taxRate / 100;

            LineTotal = taxableAmount + TaxAmount;
        }
    }
}
