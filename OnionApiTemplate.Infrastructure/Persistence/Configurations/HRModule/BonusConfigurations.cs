using Khazen.Domain.Entities.HRModule;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Khazen.Infrastructure.Persistence.Configurations.HRModule
{
    internal class BonusConfigurations : IEntityTypeConfiguration<Bonus>
    {
        public void Configure(EntityTypeBuilder<Bonus> builder)
        {
            builder.ToTable("Bonuses");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Amount).IsRequired().HasPrecision(18, 2);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.Reason).HasMaxLength(500).IsRequired(false);

            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                    d => d.ToDateTime(TimeOnly.MinValue),
                    d => DateOnly.FromDateTime(d));

            builder.Property(a => a.Date)
                .HasConversion(dateOnlyConverter);

            builder.HasOne(e => e.Employee).WithMany(e => e.Bonuses).HasForeignKey(e => e.EmployeeId).IsRequired(false);
        }
    }
}
