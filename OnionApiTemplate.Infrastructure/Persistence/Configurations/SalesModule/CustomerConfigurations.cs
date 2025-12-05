namespace Khazen.Infrastructure.Persistence.Configurations.SalesModule
{
    internal class CustomerConfigurations : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(x => x.Name).IsUnique();

            builder.HasMany(x => x.Orders).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId);
            builder.HasMany(x => x.Invoices).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).IsRequired(false);
        }
    }
}
