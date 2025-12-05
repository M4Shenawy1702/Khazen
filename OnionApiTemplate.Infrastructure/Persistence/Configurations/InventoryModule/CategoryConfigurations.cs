using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Infrastructure.Persistence.Configurations.InventoryModule
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasComment("Unique category name.");

            builder.HasIndex(x => x.Name)
                   .IsUnique();

            builder.Property(x => x.Description)
                   .HasMaxLength(500)
                   .HasComment("Optional description for the category.");

            builder.HasMany(x => x.Products)
                   .WithOne(x => x.Category)
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
