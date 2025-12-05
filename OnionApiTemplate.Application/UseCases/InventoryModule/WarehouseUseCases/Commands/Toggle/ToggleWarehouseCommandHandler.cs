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
            try
            {
                _logger.LogDebug("Start toggling warehouse with id: {Id}", request.Id);

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for toggling warehouse with id: {Id}. Errors: {Errors}", request.Id, validationResult.Errors);
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }
                var repo = _unitOfWork.GetRepository<Warehouse, Guid>();
                var warehouse = await repo.GetByIdAsync(request.Id, cancellationToken, trackChanges: true);
                if (warehouse is null)
                {
                    _logger.LogInformation("warehouse with id: {Id} not found", request.Id);
                    throw new NotFoundException<Warehouse>(request.Id);
                }
                var user = await _userManager.FindByNameAsync(request.ModifiedBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserId: {ModifiedBy}", request.ModifiedBy);
                    throw new NotFoundException<ApplicationUser>(request.ModifiedBy);
                }

                warehouse.Toggle(request.ModifiedBy);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("warehouse with id: {Id} toggled successfully , now it is: {IsDeleted}", request.Id, warehouse.IsDeleted);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error occurred while toggle warehouse.");
                throw;
            }

        }
    }
}
