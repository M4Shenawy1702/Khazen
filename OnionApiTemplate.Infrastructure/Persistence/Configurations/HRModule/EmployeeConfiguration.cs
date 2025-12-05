using Khazen.Domain.Entities.HRModule;

namespace Khazen.Infrastructure.Persistence.Configurations.HRModule
{
    internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.NationalId).IsRequired().HasMaxLength(50);
            builder.HasIndex(e => e.NationalId).IsUnique();
            builder.Property(e => e.JobTitle).HasConversion<string>().IsRequired();
            builder.Property(e => e.BaseSalary).HasPrecision(18, 2).IsRequired();
            builder.Property(e => e.HireDate).IsRequired();

            builder.HasOne(e => e.Department).WithMany(d => d.Employees).HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.User)
               .WithOne(j => j.Employee)
               .HasForeignKey<Employee>(j => j.UserId)
               .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(e => e.AttendanceRecords).WithOne(e => e.Employee).HasForeignKey(e => e.EmployeeId);
            builder.HasMany(e => e.PerformanceReviews).WithOne(e => e.Employee).HasForeignKey(e => e.EmployeeId);
            builder.HasMany(e => e.Salaries).WithOne(s => s.Employee).HasForeignKey(s => s.EmployeeId);
        }
    }
}
