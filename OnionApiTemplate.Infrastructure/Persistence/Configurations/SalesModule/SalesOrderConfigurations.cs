namespace Khazen.Infrastructure.Persistence.Configurations.SalesModule
{
    internal class SalesOrderConfigurations : IEntityTypeConfiguration<SalesOrder>
    {
        public void Configure(EntityTypeBuilder<SalesOrder> builder)
        {
            builder.ToTable("SalesOrders");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.OrderDate).IsRequired();
            builder.Property(x => x.GrandTotal).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.DiscountType).IsRequired().HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.DiscountValue).HasColumnType("decimal(18,2)").HasPrecision(18, 2);

            builder.HasMany(x => x.Items)
                .WithOne(x => x.SalesOrder)
                .HasForeignKey(x => x.SalesOrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.HasOne(x => x.Invoice).WithOne(x => x.SalesOrder).HasForeignKey<SalesInvoice>(x => x.SalesOrderId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
