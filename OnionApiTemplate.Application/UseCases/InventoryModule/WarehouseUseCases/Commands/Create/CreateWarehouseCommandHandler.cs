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
            try
            {
                _logger.LogInformation("CreateWarehouseCommandHandler started.");
                _logger.LogDebug("Creating warehouse with data: {@Warehouse}", request.Dto);

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogError("Validation failed: {@Errors}", validationResult.Errors);
                    throw new BadRequestException(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }

                var user = await _userManager.FindByNameAsync(request.CreatedBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserId: {ModifiedBy}", request.CreatedBy);
                    throw new NotFoundException<ApplicationUser>(request.CreatedBy);
                }

                var repo = _unitOfWork.GetRepository<Warehouse, Guid>();

                await ValidateDuplication(request, repo, cancellationToken);

                var entity = _mapper.Map<Warehouse>(request.Dto);
                await repo.AddAsync(entity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Warehouse {WarehouseId} created successfully.", entity.Id);

                return _mapper.Map<WarehouseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error occurred while creating warehouse.");
                throw;
            }

        }

        private async Task ValidateDuplication(CreateWarehouseCommand request, IGenericRepository<Warehouse, Guid> repo, CancellationToken cancellationToken)
        {
            if (await repo.AnyAsync(w => w.Name == request.Dto.Name, cancellationToken))
            {
                _logger.LogError("Warehouse with Name '{Name}' already exists.", request.Dto.Name);
                throw new AlreadyExistsException<Warehouse>($"with Name '{request.Dto.Name}'");
            }

            if (await repo.AnyAsync(w => w.PhoneNumber == request.Dto.PhoneNumber, cancellationToken))
            {
                _logger.LogError("Warehouse with PhoneNumber '{PhoneNumber}' already exists.", request.Dto.PhoneNumber);
                throw new AlreadyExistsException<Warehouse>($"with PhoneNumber '{request.Dto.PhoneNumber}'");
            }
        }
    }
}
