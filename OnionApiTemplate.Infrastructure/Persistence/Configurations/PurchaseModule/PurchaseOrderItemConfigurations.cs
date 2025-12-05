using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Infrastructure.Persistence.Configurations.PurchaseModule
{
    internal class PurchaseOrderItemConfigurations : IEntityTypeConfiguration<PurchaseOrderItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
        {
            builder.ToTable("PurchaseOrderItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity)
                   .IsRequired();

            builder.Property(x => x.ExpectedUnitPrice)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Ignore(x => x.TotalPrice);

            builder.HasOne(x => x.PurchaseOrder)
                   .WithMany(x => x.Items)
                   .HasForeignKey(x => x.PurchaseOrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Product)
                   .WithMany()
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
