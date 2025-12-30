using Khazen.Application.BaseSpecifications.InventoryModule.ProductSpecifications;
using Khazen.Application.Common.Interfaces;
using Khazen.Application.DOTs.InventoryModule.ProductDtos;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Update
{
    internal class UpdateProductCommandHandler(IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateProductCommand> validator,
        ILogger<UpdateProductCommandHandler> logger,
        UserManager<ApplicationUser> userManager,
          IFileService fileService
        ) : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateProductCommand> _validator = validator;
        private readonly ILogger<UpdateProductCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IFileService _fileService = fileService;

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateProduct: Process started for ID {Id} by User {UserId}",
               request.Id, request.CurrentUserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("UpdateProduct: Validation failed for ID {Id}. Errors: {Errors}",
                    request.Id, string.Join(" | ", errors));
                throw new BadRequestException(errors);
            }

            var repo = _unitOfWork.GetRepository<Product, Guid>();

            _logger.LogDebug("UpdateProduct: Fetching dependencies and checking unique constraints for {Id}", request.Id);

            var userTask = _userManager.FindByIdAsync(request.CurrentUserId);
            var productTask = repo.GetAsync(new GetProductByIdspecifications(request.Id), cancellationToken);
            var nameExistsTask = repo.AnyAsync(p => p.Name == request.Dto.Name && p.Id != request.Id, cancellationToken);
            var skuExistsTask = repo.AnyAsync(p => p.SKU == request.Dto.SKU && p.Id != request.Id, cancellationToken);

            await Task.WhenAll(userTask, productTask, nameExistsTask, skuExistsTask);

            var user = await userTask;
            if (user == null)
            {
                _logger.LogError("UpdateProduct: User with ID {Id} not found.", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            var product = await productTask;

            if (product == null)
            {
                _logger.LogError("UpdateProduct: Product with ID {Id} not found.", request.Id);
                throw new NotFoundException<Product>(request.Id);
            }

            if (await nameExistsTask)
            {
                _logger.LogWarning("UpdateProduct: Conflict detected. Name '{Name}' already exists.", request.Dto.Name);
                throw new AlreadyExistsException<Product>($"Name '{request.Dto.Name}' is taken.");
            }

            if (await skuExistsTask)
            {
                _logger.LogWarning("UpdateProduct: Conflict detected. SKU '{SKU}' already exists.", request.Dto.SKU);
                throw new AlreadyExistsException<Product>($"SKU '{request.Dto.SKU}' is taken.");
            }

            string? oldImagePath = product.Image;
            string? newImagePath = null;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("UpdateProduct: Transaction initiated for {Id}", request.Id);

            try
            {
                if (request.Dto.Image != null)
                {
                    _logger.LogInformation("UpdateProduct: New image detected. Uploading for {Id}...", request.Id);
                    newImagePath = await _fileService.SaveFileAsync(request.Dto.Image, "uploads/products", cancellationToken);
                }

                _mapper.Map(request.Dto, product);

                if (newImagePath != null)
                {
                    product.Image = newImagePath;
                }

                product.ModifiedAt = DateTime.UtcNow;
                product.ModifiedBy = user.Id;

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("UpdateProduct: Database changes committed for {Id}.", request.Id);

                if (newImagePath != null && !string.IsNullOrEmpty(oldImagePath))
                {
                    _logger.LogDebug("UpdateProduct: Deleting obsolete image: {OldPath}", oldImagePath);
                    await _fileService.DeleteFileAsync(oldImagePath);
                }

                _logger.LogInformation("UpdateProduct: Successfully updated product '{Name}' (ID: {Id})", product.Name, product.Id);
                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateProduct: Transaction failed for {Id}. Starting rollback.", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                if (newImagePath != null)
                {
                    _logger.LogWarning("UpdateProduct: Deleting orphan image after failed transaction: {NewPath}", newImagePath);
                    await _fileService.DeleteFileAsync(newImagePath);
                }

                throw;
            }
        }
    }
}
