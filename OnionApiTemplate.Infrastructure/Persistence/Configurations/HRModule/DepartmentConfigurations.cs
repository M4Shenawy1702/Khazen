using Khazen.Domain.Entities.HRModule;

namespace Khazen.Infrastructure.Persistence.Configurations.HRModule
{
    internal class DepartmentConfigurations : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(e => e.Name).IsUnique();
            builder.Property(e => e.IsDeleted).HasDefaultValue(false);

            builder.Property(e => e.Description).IsRequired().HasMaxLength(200);
        }
    }
}
