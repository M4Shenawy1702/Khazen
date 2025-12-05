using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Infrastructure.Persistence.Configurations.AccountingModule
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Balance).HasPrecision(18, 2);

            builder.Property(a => a.Code)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.Code)
       .HasComment("Unique account code (e.g., 1001 for Cash)");

            builder.HasIndex(a => new { a.ParentId, a.Name }).IsUnique();

            builder.Property(a => a.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(a => a.AccountType)
                   .IsRequired();

            builder.Property(a => a.IsDeleted)
                   .HasDefaultValue(false);

            builder.HasOne(a => a.Parent)
                   .WithMany(a => a.Children)
                   .HasForeignKey(a => a.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => a.Code).IsUnique();
            builder.HasIndex(a => a.Name);

        }
    }
}
