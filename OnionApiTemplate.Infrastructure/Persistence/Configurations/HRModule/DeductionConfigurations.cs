using Khazen.Domain.Entities.HRModule;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Khazen.Infrastructure.Persistence.Configurations.HRModule
{
    internal class DeductionConfigurations : IEntityTypeConfiguration<Deduction>
    {
        public void Configure(EntityTypeBuilder<Deduction> builder)
        {
            builder.ToTable("Deductions");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Amount).IsRequired().HasPrecision(18, 2);
            builder.Property(d => d.CreatedAt).IsRequired();
            builder.Property(d => d.Reason).HasMaxLength(500).IsRequired(false);

            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                d => d.ToDateTime(TimeOnly.MinValue),
                d => DateOnly.FromDateTime(d));

            builder.Property(a => a.Date)
                .HasConversion(dateOnlyConverter);

            builder.HasOne(d => d.Employee)
                   .WithMany(e => e.Deductions)
                   .HasForeignKey(d => d.EmployeeId)
                   .IsRequired(false);
        }
    }
}
