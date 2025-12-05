using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Infrastructure.Persistence.Configurations.PurchaseModule
{
    internal class PurchaseInvoiceConfiguration : IEntityTypeConfiguration<PurchaseInvoice>
    {
        public void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
        {
            builder.ToTable("PurchaseInvoices");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.InvoiceNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(x => x.InvoiceNumber)
                   .IsUnique();

            builder.HasIndex(x => new { x.SupplierId, x.InvoiceNumber }).IsUnique();

            builder.Property(x => x.InvoiceDate)
                   .IsRequired();

            builder.Property(x => x.InvoiceStatus)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.PaymentStatus)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.TotalAmount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.PaidAmount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.RemainingAmount)
                   .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Supplier)
                  .WithMany()
                  .HasForeignKey(x => x.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PurchaseReceipt)
                   .WithOne(r => r.Invoice)
                   .HasForeignKey<PurchaseInvoice>(p => p.PurchaseReceiptId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.JournalEntry)
                   .WithMany()
                   .HasForeignKey(x => x.JournalEntryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ReversalJournalEntry)
                   .WithMany()
                   .HasForeignKey(x => x.ReversalJournalEntryId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.Property(x => x.Notes)
                   .HasMaxLength(2000);

            builder.Property(x => x.RowVersion).IsRowVersion();

            builder.HasIndex(x => x.SupplierId);
            builder.HasIndex(x => x.PurchaseReceiptId).IsUnique();
        }
    }

}
