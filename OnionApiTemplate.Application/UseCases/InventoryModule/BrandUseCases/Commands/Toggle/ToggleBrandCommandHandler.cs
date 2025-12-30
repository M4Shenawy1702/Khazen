using Khazen.Application.BaseSpecifications.InventoryModule.BrandSpecifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Delete
{
    internal class ToggleBrandCommandHandler(IUnitOfWork _unitOfWork, ILogger<ToggleBrandCommandHandler> _logger,
        UserManager<ApplicationUser> _userManager)
        : IRequestHandler<ToggleBrandCommand, bool>
    {


        public async Task<bool> Handle(ToggleBrandCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting DeleteBrandCommandHandler for Brand ID: {BrandId}", request.Id);

            var user = await _userManager.FindByIdAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogError("User with ID '{UserId}' not found", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>($"with ID '{request.CurrentUserId}'");
            }

            try
            {
                var repo = _unitOfWork.GetRepository<Brand, Guid>();
                var brand = await repo.GetAsync(new GetBrandByIdSpecification(request.Id), cancellationToken);
                if (brand is null)
                {
                    _logger.LogWarning("Brand not found with ID: {BrandId}", request.Id);
                    throw new NotFoundException<Brand>(request.Id);
                }

                if (brand.Products?.Count > 0)
                {
                    _logger.LogWarning(
                        "Attempted to delete brand '{BrandName}' (ID: {BrandId}) which has related products.",
                        brand.Name, brand.Id);

                    throw new ConflictException(
                        $"Cannot delete brand '{brand.Name}' because it has related products.");
                }

                brand.Toggle(user.Id);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Brand '{BrandName}' (ID: {BrandId}) deleted successfully.",
                    brand.Name, brand.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error occurred while deleting brand with ID: {BrandId}",
                    request.Id);

                throw new ApplicationException("An unexpected error occurred while deleting the brand.");
            }
        }

    }
}
