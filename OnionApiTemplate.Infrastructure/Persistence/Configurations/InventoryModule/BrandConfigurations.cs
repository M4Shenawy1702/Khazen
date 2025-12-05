using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Infrastructure.Persistence.Configurations.InventoryModule
{
    internal class BrandConfigurations : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brands");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasComment("The unique name of the brand.");

            builder.HasIndex(x => x.Name)
                   .IsUnique();

            builder.Property(x => x.LogoUrl)
                   .HasMaxLength(300);

            builder.Property(x => x.WebsiteUrl)
                   .HasMaxLength(300);

            builder.Property(x => x.ContactEmail)
                   .HasMaxLength(150);

            builder.HasMany(x => x.Products)
                   .WithOne(x => x.Brand)
                   .HasForeignKey(x => x.BrandId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
