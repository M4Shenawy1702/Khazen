using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Infrastructure.Persistence.Configurations.PurchaseModule
{
    internal class PurchaseOrderConfigurations : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.ToTable("PurchaseOrders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.DeliveryDate)
                   .IsRequired();

            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.Property(x => x.Notes)
                   .HasMaxLength(1000);

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.HasMany(po => po.Items)
           .WithOne()
           .HasForeignKey(i => i.PurchaseOrderId)
           .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Supplier)
                   .WithMany(x => x.PurchaseOrders)
                   .HasForeignKey(x => x.SupplierId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
