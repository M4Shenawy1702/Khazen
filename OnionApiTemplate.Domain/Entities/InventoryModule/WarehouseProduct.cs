namespace Khazen.Domain.Entities.InventoryModule
{
    public class WarehouseProduct : BaseEntity<int>
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;

        public int QuantityInStock { get; set; }
        public int ReservedQuantity { get; set; }
    }
}