using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Domain.Entities.SalesModule
{
    public class SalesOrderItem : BaseEntity<int>
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public Guid SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public DiscountType DiscountType { get; set; } = DiscountType.None;
        public decimal DiscountValue { get; set; } = 0;

        public decimal TaxRate { get; set; } = 0;

        public decimal SubTotal => Quantity * UnitPrice;

        public decimal DiscountAmount =>
            DiscountType switch
            {
                DiscountType.Percentage => Math.Round(SubTotal * (DiscountValue / 100), 2, MidpointRounding.AwayFromZero),
                DiscountType.FixedAmount => Math.Round(DiscountValue, 2, MidpointRounding.AwayFromZero),
                _ => 0
            };

        public decimal TaxAmount
        {
            get
            {
                var taxableBase = Math.Max(0, SubTotal - DiscountAmount);
                return Math.Round(taxableBase * TaxRate / 100, 2, MidpointRounding.AwayFromZero);
            }
        }

        public decimal Total => Math.Max(0, SubTotal - DiscountAmount) + TaxAmount;
    }
}
