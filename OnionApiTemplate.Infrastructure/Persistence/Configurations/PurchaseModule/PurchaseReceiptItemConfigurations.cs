using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Infrastructure.Persistence.Configurations.PurchaseModule
{
    internal class PurchaseReceiptItemConfigurations : IEntityTypeConfiguration<PurchaseReceiptItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseReceiptItem> builder)
        {
            builder.ToTable("PurchaseReceiptItems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReceivedQuantity)
                   .IsRequired();

            builder.Property(x => x.Notes)
                   .HasMaxLength(1000);

            builder.HasOne(x => x.PurchaseReceipt)
                   .WithMany(x => x.Items)
                   .HasForeignKey(x => x.PurchaseReceiptId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Product)
                   .WithMany()
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
