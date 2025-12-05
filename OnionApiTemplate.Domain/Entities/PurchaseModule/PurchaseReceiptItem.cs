using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Domain.Entities.PurchaseModule
{
    public class PurchaseReceiptItem : BaseEntity<int>
    {
        public Guid PurchaseReceiptId { get; set; }
        public PurchaseReceipt? PurchaseReceipt { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public int ReceivedQuantity { get; set; }

        public string Notes { get; set; } = string.Empty;

    }

}