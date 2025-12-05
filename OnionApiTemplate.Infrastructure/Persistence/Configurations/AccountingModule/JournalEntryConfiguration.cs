using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Infrastructure.Persistence.Configurations.AccountingModule
{
    public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
    {
        public void Configure(EntityTypeBuilder<JournalEntry> builder)
        {
            builder.ToTable("JournalEntries");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.JournalEntryNumber)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.HasIndex(j => j.JournalEntryNumber).IsUnique();

            builder.Property(j => j.Description)
                   .HasMaxLength(1000);

            builder.Property(j => j.EntryDate)
                   .IsRequired();

            builder.Property(j => j.TransactionType)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(j => j.RelatedEntityType)
                   .HasConversion<string>();

            builder.Property(j => j.IsReversal).HasDefaultValue(false);
            builder.Property(j => j.IsReversed).HasDefaultValue(false);

            builder.Property(j => j.ReversedAt).IsRequired(false);

            builder.HasOne(j => j.ReversalOfJournalEntry)
                   .WithMany()
                   .HasForeignKey(j => j.ReversalOfJournalEntryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(j => j.Lines)
                   .WithOne(l => l.JournalEntry)
                   .HasForeignKey(l => l.JournalEntryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(j => j.EntryDate);
        }
    }

}
