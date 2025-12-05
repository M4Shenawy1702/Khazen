using Khazen.Domain.Entities.HRModule;
using static Khazen.Domain.Entities.HRModule.AttendanceRecord;

namespace Khazen.Infrastructure.Persistence.Configurations.HRModule
{
    internal class AttendanceRecordConfigurations : IEntityTypeConfiguration<AttendanceRecord>
    {
        public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
        {
            builder.ToTable("AttendanceRecords");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Date).IsRequired();
            builder.Property(e => e.CheckInTime).IsRequired(false);
            builder.Property(e => e.CheckOutTime).IsRequired(false);
            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(AttendanceStatus.Present);
            builder.Property(e => e.Notes).HasMaxLength(500).IsRequired(false);

            builder.HasOne(e => e.Employee).WithMany(e => e.AttendanceRecords).HasForeignKey(e => e.EmployeeId).IsRequired(false);
        }
    }
}
