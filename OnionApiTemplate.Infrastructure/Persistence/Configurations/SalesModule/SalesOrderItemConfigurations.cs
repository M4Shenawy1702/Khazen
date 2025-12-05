namespace Khazen.Infrastructure.Persistence.Configurations.SalesModule
{
    internal class SalesOrderItemConfigurations : IEntityTypeConfiguration<SalesOrderItem>
    {
        public void Configure(EntityTypeBuilder<SalesOrderItem> builder)
        {
            builder.ToTable("SalesOrderItems");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Quantity).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.UnitPrice).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.DiscountType).HasConversion<string>().HasMaxLength(20).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.DiscountValue).IsRequired().HasPrecision(18, 2);

            builder.HasOne(x => x.SalesOrder)
           .WithMany(x => x.Items)
           .HasForeignKey(x => x.SalesOrderId)
           .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(x => x.Total);
        }
    }
}
