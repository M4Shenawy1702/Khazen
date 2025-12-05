using Khazen.Domain.Entities.UsersModule;

namespace Khazen.Infrastructure.Persistence.Configurations.UserModule
{
    internal class RefreshTokenConfigurations : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Token).IsUnique();
            builder.Property(x => x.Token).IsRequired().HasMaxLength(200);

            builder
              .HasOne(r => r.User)
              .WithMany(u => u.RefreshTokens)
              .HasForeignKey(r => r.UserId)
              .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
