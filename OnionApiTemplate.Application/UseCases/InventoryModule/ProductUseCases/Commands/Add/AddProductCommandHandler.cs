using Khazen.Application.Common.Interfaces;
using Khazen.Application.DOTs.InventoryModule.ProductDtos;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Add
{
    public class AddProductCommandHandler
        (
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFileService fileService,
        IValidator<AddProductCommand> validator,
        ILogger<AddProductCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<AddProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IFileService _fileService = fileService;
        private readonly IValidator<AddProductCommand> _validator = validator;
        private readonly ILogger<AddProductCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<ProductDto> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AddProduct: Initiating request for '{Name}' (SKU: {SKU}) by User: {UserId}",
                request.Dto.Name, request.Dto.SKU, request.CurrentUserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("AddProduct: Validation failed for '{Name}': {Errors}",
                    request.Dto.Name, string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var repo = _unitOfWork.GetRepository<Product, Guid>();

            _logger.LogDebug("AddProduct: Performing parallel checks for dependencies and duplicates.");

            var user = await _userManager.FindByIdAsync(request.CurrentUserId);
            if (user == null)
            {
                _logger.LogError("AddProduct: Audit Error - User ID '{UserId}' not found in system.", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            var warehouseTask = _unitOfWork.GetRepository<Warehouse, Guid>().GetByIdAsync(request.Dto.WareHouseId, cancellationToken);
            var supplierTask = _unitOfWork.GetRepository<Supplier, Guid>().GetByIdAsync(request.Dto.SupplierId, cancellationToken);
            var nameExistsTask = repo.AnyAsync(p => p.Name == request.Dto.Name, cancellationToken);
            var skuExistsTask = repo.AnyAsync(p => p.SKU == request.Dto.SKU, cancellationToken);

            await Task.WhenAll(warehouseTask, supplierTask, nameExistsTask, skuExistsTask);

            if (warehouseTask.Result == null)
            {
                _logger.LogWarning("AddProduct: Warehouse ID {Id} not found.", request.Dto.WareHouseId);
                throw new NotFoundException<Warehouse>(request.Dto.WareHouseId);
            }

            if (supplierTask.Result == null)
            {
                _logger.LogWarning("AddProduct: Supplier ID {Id} not found.", request.Dto.SupplierId);
                throw new NotFoundException<Supplier>(request.Dto.SupplierId);
            }

            if (nameExistsTask.Result)
            {
                _logger.LogWarning("AddProduct: Name Conflict - '{Name}' already exists.", request.Dto.Name);
                throw new AlreadyExistsException<Product>($"Name '{request.Dto.Name}' is taken.");
            }

            if (skuExistsTask.Result)
            {
                _logger.LogWarning("AddProduct: SKU Conflict - '{SKU}' already exists.", request.Dto.SKU);
                throw new AlreadyExistsException<Product>($"SKU '{request.Dto.SKU}' is taken.");
            }

            string? imagePath = null;
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("AddProduct: Transaction started.");

            try
            {
                if (request.Dto.Image != null)
                {
                    _logger.LogDebug("AddProduct: Uploading product image.");
                    imagePath = await _fileService.SaveFileAsync(request.Dto.Image, "uploads/products", cancellationToken);
                }

                var product = _mapper.Map<Product>(request.Dto);
                product.Image = imagePath!;
                product.AvgPrice = request.Dto.PurchasePrice;
                product.CreatedBy = user.Id;
                product.CreatedAt = DateTime.UtcNow;

                product.WarehouseProducts.Add(new WarehouseProduct { WarehouseId = request.Dto.WareHouseId });
                product.SupplierProducts.Add(new SupplierProduct { SupplierId = request.Dto.SupplierId });

                await repo.AddAsync(product, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("AddProduct: Product '{Name}' successfully created with ID {Id}.",
                    product.Name, product.Id);

                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddProduct: Critical failure. Rolling back transaction for '{Name}'.", request.Dto.Name);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                if (imagePath != null)
                {
                    _logger.LogInformation("AddProduct: Cleaning up orphan file: {Path}", imagePath);
                    await _fileService.DeleteFileAsync(imagePath);
                }

                throw;
            }
        }
    }

}
