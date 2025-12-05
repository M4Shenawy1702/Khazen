using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Infrastructure.Persistence.Configurations.PurchaseModule
{
    internal class SupplierProductConfigurations : IEntityTypeConfiguration<SupplierProduct>
    {
        public void Configure(EntityTypeBuilder<SupplierProduct> builder)
        {
            builder.ToTable("SupplierProducts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PurchasePrice)
                   .HasPrecision(18, 2);

            builder.HasOne(x => x.Supplier)
                   .WithMany(x => x.SupplierProducts)
                   .HasForeignKey(x => x.SupplierId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Product)
                   .WithMany(p => p.SupplierProducts)
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.SupplierId, x.ProductId })
               .IsUnique()
               .HasDatabaseName("IX_SupplierProducts_SupplierId_ProductId");

            builder.HasIndex(x => x.ProductId)
       .IncludeProperties(x => x.PurchasePrice);
        }
    }
}
