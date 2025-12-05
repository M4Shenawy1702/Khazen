using Khazen.Application.BaseSpecifications.HRModule.PerformanceReviewSpesifications;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Update
{
    internal class UpdatePerformanceReviewCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdatePerformanceReviewCommand> validator,
        UserManager<ApplicationUser> userManager,
        ILogger<UpdatePerformanceReviewCommandHandler> logger)
        : IRequestHandler<UpdatePerformanceReviewCommand, PerformanceReviewDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdatePerformanceReviewCommand> _validator = validator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<UpdatePerformanceReviewCommandHandler> _logger = logger;

        public async Task<PerformanceReviewDto> Handle(UpdatePerformanceReviewCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting update of PerformanceReview with ID: {Id} by {ModifiedBy}", request.Id, request.ModifiedBy);

            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for PerformanceReview update. Errors: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var repo = _unitOfWork.GetRepository<PerformanceReview, Guid>();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var review = await repo.GetAsync(new GetPerformanceReviewByIdSpesification(request.Id), cancellationToken);
                if (review is null)
                {
                    _logger.LogWarning("PerformanceReview not found: {Id}", request.Id);
                    throw new NotFoundException<PerformanceReview>(request.Id);
                }

                _logger.LogDebug("Found PerformanceReview with ID: {Id}", request.Id);

                var user = await _userManager.FindByNameAsync(request.ModifiedBy);
                if (user is null)
                {
                    _logger.LogWarning("User not found: {ModifiedBy}", request.ModifiedBy);
                    throw new NotFoundException<ApplicationUser>(request.ModifiedBy);
                }

                _mapper.Map(request.Dto, review);
                review.ModifiedAt = DateTime.UtcNow;
                review.ModifiedBy = request.ModifiedBy;

                repo.Update(review);
                _logger.LogDebug("Mapped and updated PerformanceReview entity for ID: {Id}", request.Id);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Successfully updated PerformanceReview with ID: {Id}", request.Id);

                return _mapper.Map<PerformanceReviewDto>(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating PerformanceReview with ID: {Id}. Rolling back transaction.", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
