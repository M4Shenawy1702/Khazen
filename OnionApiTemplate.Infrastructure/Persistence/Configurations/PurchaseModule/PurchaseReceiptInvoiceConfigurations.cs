using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Infrastructure.Persistence.Configurations.PurchaseModule
{
    internal class PurchaseReceiptInvoiceConfigurations : IEntityTypeConfiguration<PurchaseInvoice>
    {
        public void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
        {
            builder.ToTable("PurchaseReceiptInvoices");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.InvoiceNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.InvoiceDate)
                   .IsRequired();

            builder.Property(x => x.TotalAmount)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(x => x.PaidAmount)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(x => x.RemainingAmount)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(x => x.Notes)
                   .HasMaxLength(1000);

            builder.Property(x => x.PaymentStatus)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(x => x.InvoiceStatus)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.HasOne(x => x.Supplier)
                   .WithMany()
                   .HasForeignKey(x => x.SupplierId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
