namespace Khazen.Application.DOTs.InventoryModule.WarehouseDtos
{
    public class UpdateWarehouseDto
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public ICollection<WarehouseProductDetailsDto> WarehouseProducts { get; set; } = [];
    }
}
