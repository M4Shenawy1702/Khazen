using Khazen.Application.Specification.InventoryModule.WareHouseSpesifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Delete
{
    internal class ToggleWarehouseCommandHandler(IUnitOfWork unitOfWork, ILogger<ToggleWarehouseCommandHandler> logger, IValidator<ToggleWarehouseCommand> validator, UserManager<ApplicationUser> userManager)
        : IRequestHandler<ToggleWarehouseCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ToggleWarehouseCommandHandler> _logger = logger;
        private readonly IValidator<ToggleWarehouseCommand> _validator = validator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(ToggleWarehouseCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ToggleWarehouse: Operation started for Warehouse ID: {Id} by User: {UserId}",
                request.Id, request.CurrentUserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("ToggleWarehouse: Validation failed for ID: {Id}. Errors: {Errors}",
                    request.Id, string.Join(" | ", errors));
                throw new BadRequestException(errors);
            }

            try
            {
                var repo = _unitOfWork.GetRepository<Warehouse, Guid>();

                _logger.LogDebug("ToggleWarehouse: Fetching User and Warehouse (including Stock) in parallel for {Id}", request.Id);

                var userTask = _userManager.FindByIdAsync(request.CurrentUserId);
                var warehouseTask = repo.GetAsync(new GetWareHouseByIdSpec(request.Id), cancellationToken);

                await Task.WhenAll(userTask, warehouseTask);

                var user = await userTask;
                var warehouse = await warehouseTask;

                if (user is null)
                {
                    _logger.LogError("ToggleWarehouse: Audit failure. User {UserId} not found.", request.CurrentUserId);
                    throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
                }

                if (warehouse is null)
                {
                    _logger.LogWarning("ToggleWarehouse: Warehouse {Id} not found.", request.Id);
                    throw new NotFoundException<Warehouse>(request.Id);
                }


                if (!warehouse.IsDeleted)
                {
                    _logger.LogDebug("ToggleWarehouse: Checking stock levels for Warehouse '{Name}' before deactivation.", warehouse.Name);

                    var hasStock = warehouse.WarehouseProducts?.Any(wp => wp.QuantityInStock > 0) ?? false;
                    if (hasStock)
                    {
                        var totalStock = warehouse.WarehouseProducts?.Sum(wp => wp.QuantityInStock) ?? 0;
                        _logger.LogWarning("ToggleWarehouse: Conflict - Warehouse '{Name}' ({Id}) still contains {Stock} units.",
                            warehouse.Name, warehouse.Id, totalStock);

                        throw new ConflictException($"Cannot deactivate warehouse '{warehouse.Name}' because it still contains stock ({totalStock} units).");
                    }
                }

                bool previousState = warehouse.IsDeleted;
                _logger.LogDebug("ToggleWarehouse: Toggling state for '{Name}'. Current IsDeleted: {State}",
                    warehouse.Name, previousState);

                warehouse.Toggle(user.Id);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("ToggleWarehouse: Success. Warehouse '{Name}' ({Id}) flipped from Deleted:{Old} to Deleted:{New}. Action by: {UserId}",
                   warehouse.Name, warehouse.Id, previousState, warehouse.IsDeleted, user.Id);

                return true;
            }
            catch (Exception ex) when (ex is not DomainException)
            {
                _logger.LogCritical(ex, "ToggleWarehouse: Critical system failure while toggling Warehouse {Id}", request.Id);
                throw;
            }
        }
    }
}
