using Khazen.Application.BaseSpecifications.InventoryModule.CategorySpecifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Delete
{
    internal class DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteCategoryCommandHandler> logger)
        : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<DeleteCategoryCommandHandler> _logger = logger;

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting DeleteCategoryCommandHandler for Category ID: {CategoryId}", request.Id);

            var repo = _unitOfWork.GetRepository<Category, Guid>();
            var category = await repo.GetAsync(new GetCategoryByIdSpecification(request.Id), cancellationToken);
            if (category is null)
            {
                _logger.LogWarning("Attempted to delete non-existing category with ID {CategoryId}.", request.Id);
                throw new NotFoundException<Category>(request.Id);
            }

            if (category.Products.Count > 0)
            {
                _logger.LogWarning("Cannot delete category '{CategoryName}' because it has {ProductCount} associated products.",
                    category.Name, category.Products.Count);
                throw new BadRequestException("Cannot delete category with associated products.");
            }

            repo.Delete(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Category '{CategoryName}' with ID {CategoryId} deleted successfully.", category.Name, category.Id);

            return true;
        }
    }
}
