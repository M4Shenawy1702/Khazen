using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Infrastructure.Persistence.Configurations.InventoryModule
{
    internal class WarehouseProductConfiguration : IEntityTypeConfiguration<WarehouseProduct>
    {
        public void Configure(EntityTypeBuilder<WarehouseProduct> builder)
        {
            builder.ToTable("WarehouseProducts");

            builder.HasKey(wp => wp.Id);

            builder.Property(wp => wp.QuantityInStock)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.HasOne(wp => wp.Product)
                   .WithMany(p => p.WarehouseProducts)
                   .HasForeignKey(wp => wp.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(wp => wp.Warehouse)
                   .WithMany(w => w.WarehouseProducts)
                   .HasForeignKey(wp => wp.WarehouseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(wp => new { wp.ProductId, wp.WarehouseId })
                   .IsUnique()
                   .HasDatabaseName("IX_WarehouseProduct_Unique");
        }
    }
}
