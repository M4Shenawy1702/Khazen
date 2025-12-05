using Khazen.Domain.Entities.ConfigurationModule;

namespace Khazen.Infrastructure.Persistence.Configurations.ConfigurationModule
{
    public class CompanySettingConfiguration : IEntityTypeConfiguration<CompanySetting>
    {
        public void Configure(EntityTypeBuilder<CompanySetting> builder)
        {
            builder.ToTable("CompanySettings");

            builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
            builder.Property(c => c.Address).HasMaxLength(500);
            builder.Property(c => c.DomainName).HasMaxLength(200);
            builder.Property(c => c.Phone).HasMaxLength(20);
            builder.Property(c => c.Email).HasMaxLength(100);
            builder.Property(c => c.LogoUrl).HasMaxLength(500);
            builder.Property(c => c.CurrencySymbol).HasMaxLength(10);
            builder.Property(c => c.DefaultLanguage).HasMaxLength(20);
            builder.Property(c => c.DefaultTaxRate).HasPrecision(18, 4);
        }
    }
}
