using Khazen.Application.DOTs.InventoryModule.WarehouseDtos;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Create
{
    internal class CreateWarehouseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateWarehouseCommand> validator, ILogger<CreateWarehouseCommandHandler> logger, UserManager<ApplicationUser> userManager)
        : IRequestHandler<CreateWarehouseCommand, WarehouseDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateWarehouseCommand> _validator = validator;
        private readonly ILogger<CreateWarehouseCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<WarehouseDto> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateWarehouse: Request initiated for '{Name}' by User {UserId}",
                request.Dto.Name, request.CurrentUserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("CreateWarehouse: Validation failed for '{Name}': {Errors}",
                    request.Dto.Name, string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var repo = _unitOfWork.GetRepository<Warehouse, Guid>();

            _logger.LogDebug("CreateWarehouse: Checking dependencies and unique constraints in parallel.");

            var userTask = _userManager.FindByIdAsync(request.CurrentUserId);
            var nameExistsTask = repo.AnyAsync(w => w.Name == request.Dto.Name, cancellationToken);
            var phoneExistsTask = repo.AnyAsync(w => w.PhoneNumber == request.Dto.PhoneNumber, cancellationToken);

            await Task.WhenAll(userTask, nameExistsTask, phoneExistsTask);

            var user = await userTask;
            if (user == null)
            {
                _logger.LogWarning("CreateWarehouse: User '{UserId}' not found.", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            if (await nameExistsTask)
            {
                _logger.LogWarning("CreateWarehouse: Conflict - Name '{Name}' already exists.", request.Dto.Name);
                throw new AlreadyExistsException<Warehouse>($"with Name '{request.Dto.Name}'");
            }

            if (await phoneExistsTask)
            {
                _logger.LogWarning("CreateWarehouse: Conflict - Phone '{Phone}' already exists.", request.Dto.PhoneNumber);
                throw new AlreadyExistsException<Warehouse>($"with PhoneNumber '{request.Dto.PhoneNumber}'");
            }

            try
            {
                var warehouse = _mapper.Map<Warehouse>(request.Dto);
                warehouse.CreatedBy = user.Id;
                warehouse.CreatedAt = DateTime.UtcNow;

                await repo.AddAsync(warehouse, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("CreateWarehouse: Warehouse '{Name}' (ID: {Id}) created successfully.",
                    warehouse.Name, warehouse.Id);

                return _mapper.Map<WarehouseDto>(warehouse);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "CreateWarehouse: Critical failure while saving warehouse '{Name}'.", request.Dto.Name);
                throw;
            }
        }
    }
}
