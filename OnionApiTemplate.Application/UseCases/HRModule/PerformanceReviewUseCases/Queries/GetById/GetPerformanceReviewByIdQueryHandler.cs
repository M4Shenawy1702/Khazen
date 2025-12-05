using Khazen.Application.BaseSpecifications.HRModule.PerformanceReviewSpesifications;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Queries.GetById
{
    internal class GetPerformanceReviewByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetPerformanceReviewByIdQueryHandler> logger)
        : IRequestHandler<GetPerformanceReviewByIdQuery, PerformanceReviewDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetPerformanceReviewByIdQueryHandler> _logger = logger;

        public async Task<PerformanceReviewDto> Handle(GetPerformanceReviewByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting to handle GetPerformanceReviewByIdQuery for Id: {Id}", request.Id);

            try
            {
                var performanceReviewRepository = _unitOfWork.GetRepository<PerformanceReview, Guid>();
                var specification = new GetPerformanceReviewByIdSpesification(request.Id);

                _logger.LogDebug("Fetching performance review with Id: {Id} from repository.", request.Id);

                var review = await performanceReviewRepository.GetAsync(specification, cancellationToken, true);
                if (review is null)
                {
                    _logger.LogWarning("Performance review not found for Id: {Id}.", request.Id);
                    throw new NotFoundException<PerformanceReview>(request.Id);
                }
                _logger.LogInformation("Successfully retrieved performance review with Id: {Id}", request.Id);

                return _mapper.Map<PerformanceReviewDto>(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving performance review with Id: {Id}", request.Id);
                throw;
            }
        }
    }
}
