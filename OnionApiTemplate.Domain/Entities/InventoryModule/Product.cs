using Khazen.Domain.Entities.PurchaseModule;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khazen.Domain.Entities.InventoryModule
{
    public class Product : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;

        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal AvgPrice { get; set; }

        private decimal _taxRate;
        public decimal TaxRate
        {
            get => _taxRate;
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException(nameof(TaxRate), "Tax rate must be between 0 and 100.");
                _taxRate = value;
            }
        }

        [NotMapped]
        public int QuantityInStock => WarehouseProducts?.Sum(x => x.QuantityInStock) ?? 0;

        [NotMapped]
        public int ReservedQuantity => WarehouseProducts?.Sum(x => x.ReservedQuantity) ?? 0;

        [NotMapped]
        public int AvailableQuantity => QuantityInStock - ReservedQuantity;

        public int MinimumStock { get; set; } = 0;
        public string SKU { get; set; } = string.Empty;

        public string? ToggledBy { get; set; }
        public DateTime ToggledAt { get; set; }

        public Guid BrandId { get; set; }
        public Brand? Brand { get; set; }

        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public ICollection<WarehouseProduct> WarehouseProducts { get; set; } = [];
        public ICollection<SupplierProduct> SupplierProducts { get; set; } = [];

        public void ValidateBusinessRules()
        {
            if (SellingPrice < PurchasePrice)
                throw new InvalidOperationException("Selling price cannot be lower than purchase price.");

            if (MinimumStock < 0)
                throw new InvalidOperationException("Minimum stock cannot be negative.");
        }
        public void Toggle(string toggledBy)
        {
            IsDeleted = !IsDeleted;
            ToggledBy = toggledBy;
            ToggledAt = DateTime.Now;
        }
    }
}
