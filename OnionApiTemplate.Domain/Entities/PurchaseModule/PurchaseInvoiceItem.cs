using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Domain.Entities.PurchaseModule
{
    public class PurchaseInvoiceItem
        : BaseEntity<int>
    {
        public Guid PurchaseInvoiceId { get; set; }
        public PurchaseInvoice? PurchaseInvoice { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal AdditionalCharges { get; set; } = 0;

        public decimal TotalPrice => (Quantity * UnitPrice) + AdditionalCharges;

        public string Notes { get; set; } = string.Empty;
    }
}
