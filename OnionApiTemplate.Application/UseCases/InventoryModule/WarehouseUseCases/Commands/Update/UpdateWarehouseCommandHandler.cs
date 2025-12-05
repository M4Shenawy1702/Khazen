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
            try
            {
                _logger.LogDebug("Starting handling of UpdateWarehouseCommand for Warehouse Id: {WarehouseId}", request.Id);
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for UpdateWarehouseCommand for Warehouse Id: {WarehouseId}", request.Id);
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var repo = _unitOfWork.GetRepository<Warehouse, Guid>();
                var warehouse = await repo.GetAsync(new GetWareHouseByIdSpec(request.Id), cancellationToken);
                if (warehouse == null)
                {
                    _logger.LogWarning("Warehouse not found for Id: {WarehouseId}", request.Id);
                    throw new NotFoundException<Warehouse>(request.Id);
                }

                var user = await _userManager.FindByNameAsync(request.ModifiedBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserId: {ModifiedBy}", request.ModifiedBy);
                    throw new NotFoundException<ApplicationUser>(request.ModifiedBy);
                }
                var nameExistsTask = repo.AnyAsync(w => w.Name == request.Dto.Name && w.Id != request.Id, cancellationToken);
                var phoneExistsTask = repo.AnyAsync(w => w.PhoneNumber == request.Dto.PhoneNumber && w.Id != request.Id, cancellationToken);

                await Task.WhenAll(nameExistsTask, phoneExistsTask);


                if (nameExistsTask.Result)
                {
                    _logger.LogWarning("Warehouse name already exists for Name: {Name}", request.Dto.Name);
                    throw new AlreadyExistsException<Warehouse>($"Name '{request.Dto.Name}' is already in use.");
                }
                if (phoneExistsTask.Result)
                {
                    _logger.LogWarning("Warehouse PhoneNumber already exists for PhoneNumber: {PhoneNumber}", request.Dto.PhoneNumber);
                    throw new AlreadyExistsException<Warehouse>($"PhoneNumber '{request.Dto.PhoneNumber}' is already in use.");
                }

                _mapper.Map(request.Dto, warehouse);
                warehouse.ModifiedAt = DateTime.UtcNow;
                warehouse.ModifiedBy = request.ModifiedBy;
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("UpdateWarehouseCommand for Warehouse Id: {WarehouseId} completed successfully", request.Id);
                return _mapper.Map<WarehouseDto>(warehouse);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error occurred while updating warehouse.");
                throw;
            }
        }
    }
}
