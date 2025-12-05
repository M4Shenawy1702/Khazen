using Khazen.Domain.Entities;

namespace Khazen.Infrastructure.Persistence.Configurations
{
    internal class NumberSequenceConfigurations : IEntityTypeConfiguration<NumberSequence>
    {
        public void Configure(EntityTypeBuilder<NumberSequence> builder)
        {
            builder.HasData(
                 new NumberSequence
                 {
                     Id = 1,
                     Prefix = "JE",
                     Year = DateTime.UtcNow.Year,
                     CurrentNumber = 0
                 }
             );

        }
    }
}
