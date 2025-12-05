using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Infrastructure.Persistence.Configurations.InventoryModule
{
    internal class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Description)
                   .HasMaxLength(1000);

            builder.Property(x => x.Image)
                   .HasMaxLength(300)
                   .IsRequired(false);

            builder.Property(x => x.SKU)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.PurchasePrice)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(x => x.SellingPrice)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(x => x.AvgPrice)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(x => x.TaxRate)
                   .HasPrecision(5, 2)
                   .HasDefaultValue(0);

            builder.Property(x => x.MinimumStock)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.HasOne(x => x.Brand)
                   .WithMany(i => i.Products)
                   .HasForeignKey(x => x.BrandId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Category)
                   .WithMany(i => i.Products)
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.WarehouseProducts)
                   .WithOne(x => x.Product)
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.SupplierProducts)
                   .WithOne(x => x.Product)
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.SKU).IsUnique();
            builder.HasIndex(p => p.BrandId);
            builder.HasIndex(p => p.CategoryId);
        }
    }
}
