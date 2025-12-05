using Khazen.Application.Common.Interfaces;
using Khazen.Application.DOTs.InventoryModule.ProductDtos;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Add
{
    public class AddProductCommandHandler
        (IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService, IValidator<AddProductCommand> validator, ILogger<AddProductCommandHandler> logger)
        : IRequestHandler<AddProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IFileService _fileService = fileService;
        private readonly IValidator<AddProductCommand> _validator = validator;
        private readonly ILogger<AddProductCommandHandler> _logger = logger;

        public async Task<ProductDto> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting AddProductCommandHandler for Product Name: {ProductName}", request.Dto.Name);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed: {@Errors}", validationResult.Errors);
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var warehouseRepository = await _unitOfWork.GetRepository<Warehouse, Guid>().GetByIdAsync(request.Dto.WareHouseId, cancellationToken, true)
             ?? throw new NotFoundException<Warehouse>(request.Dto.WareHouseId);
            var supplierRepository = await _unitOfWork.GetRepository<Supplier, Guid>().GetByIdAsync(request.Dto.SupplierId, cancellationToken, true)
                ?? throw new NotFoundException<Supplier>(request.Dto.SupplierId);

            var productRepository = _unitOfWork.GetRepository<Product, Guid>();

            await ValidateDuplication(request, productRepository, cancellationToken);

            var imagePath = await _fileService.SaveFileAsync(request.Dto.Image, "uploads/products", cancellationToken);

            var product = _mapper.Map<Product>(request.Dto);
            product.Image = imagePath;
            product.AvgPrice = request.Dto.PurchasePrice;
            product.WarehouseProducts.Add(new WarehouseProduct { WarehouseId = request.Dto.WareHouseId });
            product.SupplierProducts.Add(new SupplierProduct { SupplierId = request.Dto.SupplierId });

            await productRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product {ProductName} added successfully", product.Name);

            return _mapper.Map<ProductDto>(product);
        }

        private static async Task ValidateDuplication(AddProductCommand request, IGenericRepository<Product, Guid> productRepository, CancellationToken cancellationToken)
        {
            if (await productRepository.AnyAsync(p => p.Name == request.Dto.Name, cancellationToken))
                throw new AlreadyExistsException<Product>($"Product with this Name : {request.Dto.Name} already exists");

            if (await productRepository.AnyAsync(p => p.SKU == request.Dto.SKU, cancellationToken))
                throw new AlreadyExistsException<Product>($"Product with SKU : {request.Dto.SKU} :already exists");
        }
    }

}
