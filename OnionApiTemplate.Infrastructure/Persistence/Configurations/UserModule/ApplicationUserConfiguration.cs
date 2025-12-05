using Khazen.Domain.Entities.HRModule;
using Khazen.Domain.Entities.UsersModule;
using Microsoft.AspNetCore.Identity;

namespace Khazen.Infrastructure.Persistence.Configurations.UserModule
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.UserName).IsUnique();
            builder.HasIndex(x => x.Email).IsUnique();
            builder.Property(x => x.UserName).IsRequired().HasMaxLength(256);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(x => x.DateOfBirth).IsRequired();
            builder.Property(x => x.Gender).HasConversion<string>().IsRequired();
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.HasOne(u => u.Employee).WithOne(e => e.User).HasForeignKey<Employee>(e => e.UserId);

            builder.HasMany(u => u.RefreshTokens)
                .WithOne(u => u.User)
                .HasForeignKey(r => r.UserId);
        }
    }

    internal class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.ToTable("Roles");
        }
    }

    internal class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.ToTable("UsersRoles");
            builder.HasKey(x => new { x.UserId, x.RoleId });
        }
    }

    internal class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
        {
            builder.ToTable("UserClaims");
            builder.HasKey(x => x.Id);
        }
    }

    internal class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
        {
            builder.ToTable("UserLogins");
            builder.HasKey(x => new { x.LoginProvider, x.ProviderKey });
        }
    }

    internal class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
        {
            builder.ToTable("UserTokens");
            builder.HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
        }
    }

    internal class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            builder.ToTable("RoleClaims");
            builder.HasKey(x => x.Id);
        }
    }
}
