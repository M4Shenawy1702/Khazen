using Khazen.Domain.Entities.HRModule;

internal class SalaryConfigurations : IEntityTypeConfiguration<Salary>
{
    public void Configure(EntityTypeBuilder<Salary> builder)
    {
        builder.ToTable("Salaries");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.BasicSalary).IsRequired().HasPrecision(18, 2);
        builder.Property(s => s.TotalBonus).HasPrecision(18, 2);
        builder.Property(s => s.TotalDeduction).HasPrecision(18, 2);
        builder.Property(s => s.TotalAdvance).HasPrecision(18, 2);
        builder.Property(s => s.NetSalary).HasPrecision(18, 2);

        builder.Property(s => s.Notes).HasMaxLength(500);

        builder.HasOne(s => s.Employee)
            .WithMany(e => e.Salaries)
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasIndex(s => new { s.EmployeeId, s.SalaryDate }).IsUnique();
    }
}
