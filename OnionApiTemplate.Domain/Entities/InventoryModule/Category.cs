namespace Khazen.Domain.Entities.InventoryModule
{
    public class Category : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<Product> Products { get; set; } = [];
    }
}