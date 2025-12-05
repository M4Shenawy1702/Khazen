using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Infrastructure.Persistence.Configurations.AccountingModule
{
    public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
    {
        public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
        {
            builder.ToTable("JournalEntryLines");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Debit)
                    .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(j => j.Credit)
                    .HasPrecision(18, 2)
                   .IsRequired();

            builder.HasOne(j => j.JournalEntry)
                   .WithMany(j => j.Lines)
                   .HasForeignKey(j => j.JournalEntryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(j => j.Account)
                   .WithMany(a => a.JournalEntryLines)
                   .HasForeignKey(j => j.AccountId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
