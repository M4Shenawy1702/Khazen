using Khazen.Application.BaseSpecifications.InventoryModule.CategorySpecifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Delete
{
    internal class ToggleCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<ToggleCategoryCommandHandler> logger, UserManager<ApplicationUser> userManager)
        : IRequestHandler<ToggleCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ToggleCategoryCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(ToggleCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Initiating Category deletion: ID {CategoryId} by User {UserId}",
                request.Id, request.CurrentUserId);

            try
            {
                var user = await _userManager.FindByIdAsync(request.CurrentUserId);
                if (user is null)
                {
                    _logger.LogError("Deletion failed: User {UserId} not found.", request.CurrentUserId);
                    throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
                }

                var repo = _unitOfWork.GetRepository<Category, Guid>();

                var category = await repo.GetAsync(new GetCategoryByIdSpecification(request.Id), cancellationToken);

                if (category is null || category.IsDeleted)
                {
                    _logger.LogWarning("Category not found or already deleted: {CategoryId}", request.Id);
                    throw new NotFoundException<Category>(request.Id);
                }
                if (category.Products?.Count > 0)
                {
                    _logger.LogWarning("Conflict: Category '{CategoryName}' has {Count} products.",
                        category.Name, category.Products.Count);
                    throw new ConflictException($"Cannot delete category '{category.Name}' because it contains active products.");
                }

                _logger.LogDebug("Marking Category {Id} as deleted.", category.Id);
                category.Toggle(user.Id);
                repo.Update(category);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Category '{CategoryName}' successfully soft-deleted.", category.Name);
                return true;
            }
            catch (Exception ex) when (ex is not DomainException)
            {
                _logger.LogError(ex, "Unexpected error during deletion of Category {Id}", request.Id);
                throw new ApplicationException("An error occurred while processing the deletion.");
            }
        }
    }
}
