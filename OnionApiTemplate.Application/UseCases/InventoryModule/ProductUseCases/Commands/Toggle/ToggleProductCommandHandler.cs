using Khazen.Application.BaseSpecifications.InventoryModule.ProductSpecifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Delete
{
    internal class ToggleProductCommandHandler(IUnitOfWork unitOfWork, ILogger<ToggleProductCommandHandler> logger, UserManager<ApplicationUser> userManager)
        : IRequestHandler<ToggleProductCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ToggleProductCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(ToggleProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ToggleProduct: Request for ID {Id} by User {UserId}", request.Id, request.CurrentUserId);

            var repo = _unitOfWork.GetRepository<Product, Guid>();

            var userTask = _userManager.FindByIdAsync(request.CurrentUserId);
            var productTask = repo.GetAsync(new GetProductByIdspecifications(request.Id), cancellationToken);

            await Task.WhenAll(userTask, productTask);

            var product = productTask.Result;
            if (product == null)
            {
                _logger.LogWarning("ToggleProduct: Failed. Product {Id} not found.", request.Id);
                throw new NotFoundException<Product>(request.Id);
            }
            var user = userTask.Result;
            if (user == null)
            {
                _logger.LogWarning("ToggleProduct: Failed. User {Id} not found.", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            var totalStock = product.WarehouseProducts?.Sum(wp => wp.QuantityInStock) ?? 0;

            if (totalStock > 0)
            {
                _logger.LogWarning("ToggleProduct: Failed. Product {Name} has {Stock} units in stock.", product.Name, totalStock);
                throw new ConflictException($"Cannot delete product '{product.Name}' because it has {totalStock} units in stock. Zero the stock first.");
            }

            try
            {
                _logger.LogDebug("ToggleProduct: Updating status for {Id}", product.Id);

                product.Toggle(user.Id);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("ToggleProduct: Product '{Name}' ({Id}) status updated successfully.", product.Name, product.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ToggleProduct: Critical error for Product {Id}", request.Id);
                throw new ApplicationException("An unexpected error occurred while updating the product status.");
            }
        }
    }
}
