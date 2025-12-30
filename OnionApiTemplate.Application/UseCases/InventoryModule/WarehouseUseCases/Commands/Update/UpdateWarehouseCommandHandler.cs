using Khazen.Application.DOTs.InventoryModule.WarehouseDtos;
using Khazen.Application.Specification.InventoryModule.WareHouseSpesifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Update
{
    internal class UpdateWarehouseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateWarehouseCommand> validator,
        ILogger<UpdateWarehouseCommandHandler> logger, UserManager<ApplicationUser> userManager)
        : IRequestHandler<UpdateWarehouseCommand, WarehouseDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateWarehouseCommand> _validator = validator;
        private readonly ILogger<UpdateWarehouseCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<WarehouseDto> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateWarehouse: Request received for Warehouse ID: {Id} by User: {UserId}",
               request.Id, request.CurrentUserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("UpdateWarehouse: Validation failed for ID: {Id}. Reasons: {Errors}",
                    request.Id, string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            try
            {
                var repo = _unitOfWork.GetRepository<Warehouse, Guid>();

                _logger.LogDebug("UpdateWarehouse: Fetching record and checking unique constraints in parallel for {Id}", request.Id);

                var userTask = _userManager.FindByIdAsync(request.CurrentUserId);
                var warehouseTask = repo.GetAsync(new GetWareHouseByIdSpec(request.Id), cancellationToken);
                var nameExistsTask = repo.AnyAsync(w => w.Name == request.Dto.Name && w.Id != request.Id, cancellationToken);
                var phoneExistsTask = repo.AnyAsync(w => w.PhoneNumber == request.Dto.PhoneNumber && w.Id != request.Id, cancellationToken);

                await Task.WhenAll(userTask, warehouseTask, nameExistsTask, phoneExistsTask);

                var user = await userTask;
                if (user is null)
                {
                    _logger.LogError("UpdateWarehouse: Audit failure. Authenticated User {UserId} not found in database.", request.CurrentUserId);
                    throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
                }

                var warehouse = await warehouseTask;
                if (warehouse is null)
                {
                    _logger.LogWarning("UpdateWarehouse: Record not found for ID {Id}.", request.Id);
                    throw new NotFoundException<Warehouse>(request.Id);
                }

                if (await nameExistsTask)
                {
                    _logger.LogWarning("UpdateWarehouse: Name Conflict. '{Name}' is already assigned to another warehouse.", request.Dto.Name);
                    throw new AlreadyExistsException<Warehouse>($"Name '{request.Dto.Name}' is already in use.");
                }

                if (await phoneExistsTask)
                {
                    _logger.LogWarning("UpdateWarehouse: Phone Conflict. '{Phone}' is already assigned to another warehouse.", request.Dto.PhoneNumber);
                    throw new AlreadyExistsException<Warehouse>($"PhoneNumber '{request.Dto.PhoneNumber}' is already in use.");
                }

                _logger.LogDebug("UpdateWarehouse: Mapping DTO changes to Entity for '{Name}' (ID: {Id})", warehouse.Name, warehouse.Id);

                var oldName = warehouse.Name;
                _mapper.Map(request.Dto, warehouse);

                warehouse.ModifiedAt = DateTime.UtcNow;
                warehouse.ModifiedBy = user.Id;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("UpdateWarehouse: Success. Warehouse '{OldName}' updated to '{NewName}' (ID: {Id}) by User {UserId}",
                   oldName, warehouse.Name, warehouse.Id, user.Id);

                return _mapper.Map<WarehouseDto>(warehouse);
            }
            catch (Exception ex) when (ex is not DomainException)
            {
                _logger.LogCritical(ex, "UpdateWarehouse: Unhandled infrastructure failure during update for ID {Id}", request.Id);
                throw;
            }
        }
    }
}
