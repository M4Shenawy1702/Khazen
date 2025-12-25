using Khazen.Application.DOTs.HRModule.BonusDtos;
using Khazen.Application.Specification.HRModule.BounsSpecifications;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.BonusUseCases.Queries.GetById
{
    internal class GetBonusByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetBonusByIdQueryHandler> logger)
        : IRequestHandler<GetBonusByIdQuery, BonusDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetBonusByIdQueryHandler> _logger = logger;

        public async Task<BonusDetailsDto> Handle(GetBonusByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Starting GetBonusByIdQueryHandler for BonusId: {BonusId}", request.Id);

                var bonusRepo = _unitOfWork.GetRepository<Bonus, Guid>();
                var bonus = await bonusRepo.GetAsync(new GetBounsByIdSpecification(request.Id), cancellationToken);

                if (bonus is null)
                {
                    _logger.LogWarning("Bonus record not found for Id: {BonusId}", request.Id);
                    throw new NotFoundException<Bonus>(request.Id);
                }

                _logger.LogInformation("Successfully retrieved bonus record for BonusId: {BonusId}, EmployeeId: {EmployeeId}",
                    request.Id, bonus.EmployeeId);

                return _mapper.Map<BonusDetailsDto>(bonus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving bonus record with Id: {BonusId}", request.Id);
                throw;
            }
        }
    }
}
