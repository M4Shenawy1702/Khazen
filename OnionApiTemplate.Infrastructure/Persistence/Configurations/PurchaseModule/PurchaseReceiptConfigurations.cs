using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Infrastructure.Persistence.Configurations.PurchaseModule
{
    internal class PurchaseReceiptConfigurations : IEntityTypeConfiguration<PurchaseReceipt>
    {
        public void Configure(EntityTypeBuilder<PurchaseReceipt> builder)
        {
            builder.ToTable("PurchaseReceipts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReceiptDate)
                   .IsRequired();

            builder.Property(x => x.Notes)
                   .HasMaxLength(1000);

            builder.Property(x => x.ReceiptNumber)
               .IsRequired()
               .HasMaxLength(50);

            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasOne(x => x.PurchaseOrder)
                   .WithMany(x => x.PurchaseReceipts)
                   .HasForeignKey(x => x.PurchaseOrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Warehouse)
                   .WithMany()
                   .HasForeignKey(x => x.WarehouseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Invoice)
                   .WithOne(i => i.PurchaseReceipt)
                   .HasForeignKey<PurchaseReceipt>(r => r.InvoiceId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => x.PurchaseOrderId);
            builder.HasIndex(x => x.WarehouseId);
            builder.HasIndex(x => x.ReceiptDate);
        }
    }

}
