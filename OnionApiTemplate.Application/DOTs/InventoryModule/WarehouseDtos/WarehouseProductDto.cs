namespace Khazen.Application.DOTs.InventoryModule.WarehouseDtos
{
    public class UpdateWarehouseProductDto
    {
        public Guid ProductId { get; set; }

        public Guid WarehouseId { get; set; }

        public int QuantityInStock { get; set; }
        public int ReservedQuantity { get; set; }
    }
}
