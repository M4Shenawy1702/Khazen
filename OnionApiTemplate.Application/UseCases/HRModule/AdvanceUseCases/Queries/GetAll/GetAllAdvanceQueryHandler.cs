using Khazen.Application.BaseSpecifications.HRModule.AdvanceSpecifications;
using Khazen.Application.DOTs.HRModule.AdvanceDtos;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AdvanceUseCases.Queries.GetAll
{
    internal class GetAllAdvanceQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<GetAllAdvanceQuery> validator,
        ILogger<GetAllAdvanceQueryHandler> logger
    ) : IRequestHandler<GetAllAdvanceQuery, IEnumerable<AdvanceDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetAllAdvanceQuery> _validator = validator;
        private readonly ILogger<GetAllAdvanceQueryHandler> _logger = logger;

        public async Task<IEnumerable<AdvanceDto>> Handle(GetAllAdvanceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Starting GetAllAdvanceQueryHandler with parameters: {@QueryParameters}", request.QueryParameters);

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for GetAllAdvanceQuery: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                    throw new BadRequestException(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }

                var advanceRepo = _unitOfWork.GetRepository<Advance, int>();
                var advances = await advanceRepo.GetAllAsync(
                    new GetAllAdvanceSpecification(request.QueryParameters),
                    cancellationToken,
                    true
                );

                _logger.LogInformation("Retrieved {Count} advances successfully.", advances.Count());

                var mappedAdvances = _mapper.Map<IEnumerable<AdvanceDto>>(advances);

                _logger.LogDebug("Successfully mapped {Count} advances to DTOs.", mappedAdvances.Count());

                return mappedAdvances;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all advances.");
                throw new ApplicationException("An unexpected error occurred while retrieving advances.", ex);
            }
        }
    }
}
