//using Khazen.Domain.Entities;

//namespace Khazen.Infrastructure.Persistence.Configurations.SafeModule
//{
//    public class SafeTransactionConfiguration : IEntityTypeConfiguration<SafeTransaction>
//    {
//        public void Configure(EntityTypeBuilder<SafeTransaction> builder)
//        {
//            builder.ToTable("SafeTransactions");

//            builder.HasKey(x => x.Id);

//            builder.Property(x => x.Date)
//                .IsRequired()
//                .HasDefaultValueSql("GETUTCDATE()");

//            builder.Property(x => x.Amount)
//                .IsRequired()
//                .HasColumnType("decimal(18,4)")
//                .HasPrecision(18, 4);

//            builder.Property(x => x.Type)
//                .IsRequired()
//                .HasConversion<string>();

//            builder.Property(x => x.Note)
//                .HasMaxLength(500);

//            builder.Property(x => x.CreatedBy)
//                .HasMaxLength(100);

//            builder.HasOne(x => x.Safe)
//                .WithMany(sf => sf.SafeTransactions)
//                .HasPrincipalKey(x => x.Id)
//                .HasForeignKey(x => x.SafeId)
//                .OnDelete(DeleteBehavior.Restrict)
//                .IsRequired(false);

//        }
//    }
//}