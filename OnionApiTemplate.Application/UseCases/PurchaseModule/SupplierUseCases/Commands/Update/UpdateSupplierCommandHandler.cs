using Khazen.Application.BaseSpecifications.PurchaseModule.SupplierSpecifications;
using Khazen.Application.DOTs.PurchaseModule.SupplierDtos;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Update
{
    internal class UpdateSupplierCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdateSupplierCommand> validator,
    UserManager<ApplicationUser> userManager,
    ILogger<UpdateSupplierCommandHandler> logger)
    : IRequestHandler<UpdateSupplierCommand, SupplierDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateSupplierCommand> _validator = validator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<UpdateSupplierCommandHandler> _logger = logger;

        public async Task<SupplierDto> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

                var user = await _userManager.FindByNameAsync(request.CurrentUserId);
                if (user is null)
                {
                    _logger.LogInformation("User {User} not found", request.CurrentUserId);
                    throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
                }

                _logger.LogInformation("User {User} is updating supplier {SupplierId}", request.CurrentUserId, request.Id);

                var repo = _unitOfWork.GetRepository<Supplier, Guid>();
                var supplier = await repo.GetAsync(new GetSupplierByIdSpecification(request.Id), cancellationToken);

                if (supplier is null)
                {
                    _logger.LogInformation("Supplier {SupplierId} not found", request.Id);
                    throw new NotFoundException<Supplier>(request.Id);
                }

                var nameConflict = await repo.GetAsync(new GetSupplierByNameSpecification(request.Dto.Name), cancellationToken, true);
                if (nameConflict is not null && nameConflict.Id != supplier.Id)
                    throw new AlreadyExistsException<Supplier>($"with Name '{request.Dto.Name}'");

                var phoneConflict = await repo.GetAsync(new GetSupplierByPhoneNumberSpecification(request.Dto.PhoneNumber), cancellationToken, true);
                if (phoneConflict is not null && phoneConflict.Id != supplier.Id)
                    throw new AlreadyExistsException<Supplier>($"with PhoneNumber '{request.Dto.PhoneNumber}'");

                var emailConflict = await repo.GetAsync(new GetSupplierByEmailSpecification(request.Dto.Email!), cancellationToken, true);
                if (emailConflict is not null && emailConflict.Id != supplier.Id)
                    throw new AlreadyExistsException<Supplier>($"with Email '{request.Dto.Email}'");

                _mapper.Map(request.Dto, supplier);
                supplier.ModifiedBy = user.Id;
                supplier.ModifiedAt = DateTime.UtcNow;

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Supplier {SupplierId} updated successfully by {User}", supplier.Id, request.CurrentUserId);

                return _mapper.Map<SupplierDto>(supplier);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error updating supplier {SupplierId} by {User}", request.Id, request.CurrentUserId);
                throw;
            }
        }
    }
}
