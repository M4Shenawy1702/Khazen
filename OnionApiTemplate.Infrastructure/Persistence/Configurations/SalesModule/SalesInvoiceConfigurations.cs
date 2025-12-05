namespace Khazen.Infrastructure.Persistence.Configurations.SalesModule
{
    internal class SalesInvoiceConfigurations : IEntityTypeConfiguration<SalesInvoice>
    {
        public void Configure(EntityTypeBuilder<SalesInvoice> builder)
        {
            builder.ToTable("SalesInvoices");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SubTotal).HasPrecision(18, 2);
            builder.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            builder.Property(x => x.TaxAmount).HasPrecision(18, 2);

            builder.Property(x => x.InvoiceNumber)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(x => x.InvoiceDate)
                   .IsRequired();
            builder.Property(x => x.Notes)
                   .HasMaxLength(1000);

            builder.Property(x => x.GrandTotal)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.HasOne(x => x.SalesOrder)
                   .WithOne(x => x.Invoice)
                   .HasForeignKey<SalesInvoice>(x => x.SalesOrderId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired(true);

            builder.HasOne(x => x.Customer)
                   .WithMany(x => x.Invoices)
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder
                 .HasOne(s => s.JournalEntry)
                .WithOne()
                .HasForeignKey<SalesInvoice>(s => s.JournalEntryId)
                .OnDelete(DeleteBehavior.Restrict);

            ;

        }
    }
}
