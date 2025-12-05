using Khazen.Application.Common.Interfaces;
using Khazen.Domain.Entities;

namespace Khazen.Application.Common.Services
{
    public class NumberSequenceService(IUnitOfWork unitOfWork)
        : INumberSequenceService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<string> GetNextNumber(string prefix, int year, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<NumberSequence, int>();

            var sequence = await repo.FirstOrDefaultAsync(x => x.Prefix == prefix && x.Year == year, cancellationToken);

            if (sequence == null)
            {
                sequence = new NumberSequence
                {
                    Prefix = prefix,
                    Year = year,
                    CurrentNumber = 0
                };
                await repo.AddAsync(sequence, cancellationToken);
            }

            sequence.CurrentNumber++;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return $"{prefix}-{year}-{sequence.CurrentNumber:D4}";
        }
    }

}
