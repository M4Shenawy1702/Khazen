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
            _logger.LogInformation("Attempting to update Performance Review: {Id}. Initiated by: {User}",
               request.Id, request.CurrentUserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed for updating Review {Id}. Errors: {Errors}", request.Id, errors);
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogWarning("User not found: {ModifiedBy}", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            var repo = _unitOfWork.GetRepository<PerformanceReview, Guid>();
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var review = await repo.GetAsync(new GetPerformanceReviewByIdSpesification(request.Id), cancellationToken);
                if (review is null)
                {
                    _logger.LogWarning("Update failed: Performance Review {Id} was not found.", request.Id);
                    throw new NotFoundException<PerformanceReview>(request.Id);
                }

                _logger.LogDebug("Mapping update DTO to Performance Review entity for ID: {Id}", request.Id);

                _mapper.Map(request.Dto, review);
                review.ModifiedAt = DateTime.UtcNow;
                review.ModifiedBy = user.UserName!;

                repo.Update(review);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Successfully updated and committed Performance Review {Id}.", request.Id);

                return _mapper.Map<PerformanceReviewDto>(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical failure during update of Performance Review {Id}. Rolling back transaction.", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
