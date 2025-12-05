using Khazen.Domain.Entities.ConfigurationModule;

namespace Khazen.Infrastructure.Persistence.Configurations.ConfigurationModule
{
    public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
    {
        public void Configure(EntityTypeBuilder<SystemSetting> builder)
        {
            builder.ToTable("SystemSettings");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Key)
                   .HasMaxLength(100)
                   .IsRequired();

            builder
                .HasIndex(x => x.Key)
                .IsUnique();


            builder.Property(s => s.Value)
                   .HasMaxLength(1000)
                   .IsRequired();

            builder.Property(s => s.Description)
                   .HasMaxLength(500);

            builder.Property(s => s.Group)
                   .HasConversion<int>()
                   .IsRequired();
        }
    }
}
