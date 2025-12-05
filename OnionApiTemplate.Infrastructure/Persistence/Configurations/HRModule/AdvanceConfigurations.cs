using Khazen.Domain.Entities.HRModule;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Khazen.Infrastructure.Persistence.Configurations.HRModule
{
    internal class AdvanceConfigurations : IEntityTypeConfiguration<Advance>
    {
        public void Configure(EntityTypeBuilder<Advance> builder)
        {
            builder.ToTable("Advances");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(a => a.Reason)
            .IsRequired()
            .HasMaxLength(500);

            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                d => d.ToDateTime(TimeOnly.MinValue),
                d => DateOnly.FromDateTime(d));

            builder.Property(a => a.Date)
               .IsRequired()
               .HasConversion(dateOnlyConverter);

            builder.Property(a => a.Date)
           .HasConversion(dateOnlyConverter)
           .HasDefaultValueSql("GETDATE()");

            builder.HasOne(a => a.Employee)
                .WithMany(e => e.Advances)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => a.EmployeeId);
            builder.HasIndex(a => a.Date);

        }
    }
}
