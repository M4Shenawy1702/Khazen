using Khazen.Application.BaseSpecifications.HRModule.PerformanceReviewSpesifications;
using Khazen.Application.Common;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Queries.GetAll
{
    internal class GetAllPerformanceReviewsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<GetAllPerformanceReviewsQuery> validator,
        ILogger<GetAllPerformanceReviewsQueryHandler> logger)
        : IRequestHandler<GetAllPerformanceReviewsQuery, PaginatedResult<PerformanceReviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetAllPerformanceReviewsQuery> _validator = validator;
        private readonly ILogger<GetAllPerformanceReviewsQueryHandler> _logger = logger;

        public async Task<PaginatedResult<PerformanceReviewDto>> Handle(GetAllPerformanceReviewsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting to handle GetAllPerformanceReviewsQuery with parameters: {@QueryParameters}", request.QueryParameters);

            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for GetAllPerformanceReviewsQuery: {@Errors}", validationResult.Errors);
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            try
            {
                var performanceReviewRepository = _unitOfWork.GetRepository<PerformanceReview, Guid>();
                var specification = new GetAllPerformanceReviewsSpesification(request.QueryParameters);

                _logger.LogDebug("Fetching performance reviews from repository with applied filters.");

                var reviews = await performanceReviewRepository.GetAllAsync(specification, cancellationToken, true);
                var data = _mapper.Map<IEnumerable<PerformanceReviewDto>>(reviews);

                var count = await performanceReviewRepository.GetCountAsync(specification, cancellationToken);

                _logger.LogInformation("Successfully retrieved {Count} performance reviews.", count);

                return new PaginatedResult<PerformanceReviewDto>(
                    request.QueryParameters.PageIndex,
                    request.QueryParameters.PageSize,
                    count,
                    data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching performance reviews.");
                throw;
            }
        }
    }
}
