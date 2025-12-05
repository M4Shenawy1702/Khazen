using Khazen.Domain.Entities.HRModule;

namespace Khazen.Infrastructure.Persistence.Configurations.HRModule
{
    internal class PerformanceReviewConfigurations : IEntityTypeConfiguration<PerformanceReview>
    {
        public void Configure(EntityTypeBuilder<PerformanceReview> builder)
        {
            builder.ToTable("PerformanceReviews");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.Comments).IsRequired(false).HasMaxLength(1000);
            builder.Property(e => e.ActionPlan).IsRequired(false).HasMaxLength(1000);
            builder.Property(e => e.Rate).IsRequired().HasDefaultValue(0);

            builder.HasOne(e => e.Employee)
                   .WithMany(e => e.PerformanceReviews)
                   .HasForeignKey(e => e.EmployeeId)
                   .IsRequired(false);

            builder.HasOne(p => p.Reviewer)
                   .WithMany()
                   .HasForeignKey(p => p.ReviewerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.ReviewDate)
                   .HasConversion(
                       v => v.ToDateTime(TimeOnly.MinValue),
                       v => DateOnly.FromDateTime(v)
                   );

        }
    }
}
