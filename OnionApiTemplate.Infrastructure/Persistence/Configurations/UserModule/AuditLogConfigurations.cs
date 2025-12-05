using Khazen.Domain.Entities.UsersModule;

namespace Khazen.Infrastructure.Persistence.Configurations.UserModule
{
    internal class AuditLogConfigurations : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.UserId);
            builder.Property(x => x.UserId).IsRequired(false);
            builder.Property(x => x.Action).HasConversion<string>().IsRequired().HasMaxLength(50);
            builder.Property(x => x.ModuleName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.EntityName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.OldValues).HasMaxLength(2000);
            builder.Property(x => x.NewValues).HasMaxLength(2000);
            builder.Property(x => x.TimeStamp).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
