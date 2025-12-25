using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Delete
{
    internal class TogglePerformanceReviewCommandHandler(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ILogger<TogglePerformanceReviewCommandHandler> logger)
        : IRequestHandler<TogglePerformanceReviewCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<TogglePerformanceReviewCommandHandler> _logger = logger;

        public async Task<bool> Handle(TogglePerformanceReviewCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting toggle operation for PerformanceReview ID: {Id} by {ModifiedBy}", request.Id, request.CurrentUserId);

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogWarning("User not found: {ModifiedBy}", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            try
            {
                var repo = _unitOfWork.GetRepository<PerformanceReview, Guid>();

                var review = await repo.GetByIdAsync(request.Id, cancellationToken);
                if (review is null)
                {
                    _logger.LogWarning("PerformanceReview not found with ID: {Id}", request.Id);
                    throw new NotFoundException<PerformanceReview>(request.Id);
                }

                review.Toggle(user.UserName!);
                _logger.LogDebug("Toggled PerformanceReview ID: {Id}", request.Id);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Successfully toggled PerformanceReview ID: {Id}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling PerformanceReview ID: {Id}. Rolling back transaction.", request.Id);
                throw;
            }
        }
    }
}
