using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Domain.Entities.PurchaseModule
{
    public class SupplierProduct
        : BaseEntity<Guid>
    {
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public decimal? PurchasePrice { get; set; }
    }

}