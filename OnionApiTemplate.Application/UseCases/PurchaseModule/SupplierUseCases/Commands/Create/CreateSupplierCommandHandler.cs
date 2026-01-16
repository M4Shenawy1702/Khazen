using Khazen.Application.BaseSpecifications.PurchaseModule.SupplierSpecifications;
using Khazen.Application.DOTs.PurchaseModule.SupplierDtos;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Create
{
    internal class CreateSupplierCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateSupplierCommand> validator,
        UserManager<ApplicationUser> userManager,
        ILogger<CreateSupplierCommandHandler> logger
    ) : IRequestHandler<CreateSupplierCommand, SupplierDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateSupplierCommand> _validator = validator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<CreateSupplierCommandHandler> _logger = logger;

        public async Task<SupplierDto> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting CreateSupplierCommand for Name: {SupplierName}", request.Dto.Name);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed for CreateSupplierCommand: {Errors}", string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var user = await _userManager.FindByNameAsync(request.CurrentUserId);
                if (user is null)
                {
                    _logger.LogError("User not found. UserId: {CurrentUserId}", request.CurrentUserId);
                    throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
                }

                var repo = _unitOfWork.GetRepository<Supplier, Guid>();

                if (await repo.GetAsync(new GetSupplierByNameSpecification(request.Dto.Name), cancellationToken) is not null)
                {
                    _logger.LogWarning("Supplier creation failed. Duplicate Name: {SupplierName}", request.Dto.Name);
                    throw new AlreadyExistsException<Supplier>($"with Name '{request.Dto.Name}'");
                }

                if (await repo.GetAsync(new GetSupplierByPhoneNumberSpecification(request.Dto.PhoneNumber), cancellationToken) is not null)
                {
                    _logger.LogWarning("Supplier creation failed. Duplicate PhoneNumber: {PhoneNumber}", request.Dto.PhoneNumber);
                    throw new AlreadyExistsException<Supplier>($"with PhoneNumber '{request.Dto.PhoneNumber}'");
                }

                if (!string.IsNullOrWhiteSpace(request.Dto.Email) &&
                    await repo.GetAsync(new GetSupplierByEmailSpecification(request.Dto.Email!), cancellationToken) is not null)
                {
                    _logger.LogWarning("Supplier creation failed. Duplicate Email: {Email}", request.Dto.Email);
                    throw new AlreadyExistsException<Supplier>($"with Email '{request.Dto.Email}'");
                }

                var entity = _mapper.Map<Supplier>(request.Dto);
                entity.CreatedBy = user.Id;

                await repo.AddAsync(entity, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Supplier created successfully. Id: {SupplierId}, Name: {SupplierName}",
                    entity.Id, entity.Name);

                return _mapper.Map<SupplierDto>(entity);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Failed to create supplier. Name: {SupplierName}", request.Dto.Name);
                throw;
            }
        }
    }
}
