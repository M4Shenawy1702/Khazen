//using Khazen.Domain.Entities;

//namespace Khazen.Infrastructure.Persistence.Configurations.SafeModule
//{
//    internal class SafeConfigutarion : IEntityTypeConfiguration<Safe>
//    {
//        public void Configure(EntityTypeBuilder<Safe> builder)
//        {
//            builder.ToTable("Safes");

//            builder.HasKey(x => x.Id);

//            builder.Property(x => x.Name)
//                .IsRequired()

//                .HasMaxLength(100);
//            builder.HasIndex(x => x.Name)
//                .IsUnique();

//            builder.Property(x => x.Description).IsRequired()
//                .HasMaxLength(500);

//            builder.Property(x => x.Type).HasConversion<string>().IsRequired();

//            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
//            builder.Property(x => x.CreatedAt)
//                .IsRequired()
//                .HasDefaultValueSql("GETDATE()");

//            builder.Property(x => x.Balance).IsRequired().HasPrecision(18, 2);

//            builder.HasMany(x => x.SafeTransactions)
//                .WithOne(x => x.Safe)
//                .HasPrincipalKey(x => x.Id)
//                .HasForeignKey(x => x.SafeId)
//                .OnDelete(DeleteBehavior.Cascade)
//                .IsRequired(false);

//        }
//    }
//}
