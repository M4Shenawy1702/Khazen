using Khazen.Application.BaseSpecifications.HRModule.BounsSpecifications;
using Khazen.Application.Common;
using Khazen.Application.DOTs.HRModule.BonusDtos;
using Khazen.Application.Specification.HRModule.BounsSpecifications;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.BonusUseCases.Queries.GetAll
{
    internal class GetAllBounsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllBounsQueryHandler> logger,
        IValidator<GetAllBounsQuery> validator
    ) : IRequestHandler<GetAllBounsQuery, PaginatedResult<BonusDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllBounsQueryHandler> _logger = logger;
        private readonly IValidator<GetAllBounsQuery> _validator = validator;

        public async Task<PaginatedResult<BonusDto>> Handle(GetAllBounsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all bonuses with query parameters: {@QueryParameters}", request.QueryParameters);

            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed . Errors: {Errors}",
                                              string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }
                var bonusRepo = _unitOfWork.GetRepository<Bonus, Guid>();

                var bonuses = await bonusRepo.GetAllAsync(new GetAllBounsSpecification(request.QueryParameters), cancellationToken, true);
                var data = _mapper.Map<IEnumerable<BonusDto>>(bonuses);

                var count = await bonusRepo.GetCountAsync(new GetAllBounsCountSpecification(), cancellationToken);
                _logger.LogInformation("Successfully retrieved {Count} bonus records.", data.Count());

                return new PaginatedResult<BonusDto>(request.QueryParameters.PageIndex, request.QueryParameters.PageSize, count, data); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching bonus records.");
                throw;
            }
        }
    }
}
