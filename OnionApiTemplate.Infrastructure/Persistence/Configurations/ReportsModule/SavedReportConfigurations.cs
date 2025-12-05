using Khazen.Domain.Entities.ReportsModule;

namespace Khazen.Infrastructure.Persistence.Configurations.ReportsModule
{
    internal class SavedReportConfigurations : IEntityTypeConfiguration<SavedReport>
    {
        public void Configure(EntityTypeBuilder<SavedReport> builder)
        {
            builder.ToTable("SavedReports");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Description)
                   .HasMaxLength(1000);

            builder.Property(x => x.ReportType)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(x => x.StartDate)
                   .IsRequired();

            builder.Property(x => x.EndDate)
                   .IsRequired();

            builder.HasOne(x => x.Employee)
                   .WithMany(i => i.SavedReports)
                   .HasForeignKey(x => x.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired(false);
        }
    }
}
