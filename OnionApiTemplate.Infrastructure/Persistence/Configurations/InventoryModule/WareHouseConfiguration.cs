using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Infrastructure.Persistence.Configurations.InventoryModule
{
    internal class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(w => w.Address)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(w => w.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.HasIndex(w => w.Name)
                   .IsUnique()
                   .HasDatabaseName("IX_Warehouse_Name_Unique");

            builder.HasMany(w => w.WarehouseProducts)
                   .WithOne(wp => wp.Warehouse)
                   .HasForeignKey(wp => wp.WarehouseId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
