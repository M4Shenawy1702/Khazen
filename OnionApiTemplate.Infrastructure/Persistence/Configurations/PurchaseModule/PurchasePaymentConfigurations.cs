using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Infrastructure.Persistence.Configurations.PurchaseModule
{
    internal class PurchasePaymentConfigurations : IEntityTypeConfiguration<PurchasePayment>
    {
        public void Configure(EntityTypeBuilder<PurchasePayment> builder)
        {
            builder.ToTable("PurchasePayments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Amount).IsRequired().HasPrecision(18, 2);
            builder
            .HasOne(p => p.PurchaseInvoice)
            .WithMany(i => i.Payments)
            .HasForeignKey(p => p.PurchaseInvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
