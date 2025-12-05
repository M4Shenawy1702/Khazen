using Khazen.Application.BaseSpecifications.InventoryModule.ProductSpecifications;
using Khazen.Application.DOTs.InventoryModule.ProductDtos;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Update
{
    internal class UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UpdateProductCommand> validator, ILogger<UpdateProductCommandHandler> logger) : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateProductCommand> _validator = validator;
        private readonly ILogger<UpdateProductCommandHandler> _logger = logger;

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UpdateProductCommandHandler started");
                _logger.LogDebug("Updating product with data: {@Product}", request.Dto);

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogError("Validation failed: {@Errors}", validationResult.Errors);
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var productRepository = _unitOfWork.GetRepository<Product, Guid>();

                var product = await productRepository.GetAsync(new GetProductByIdspecifications(request.Id), cancellationToken);
                if (product == null)
                {
                    _logger.LogError("Product not found with id: {Id}", request.Id);
                    throw new NotFoundException<Product>(request.Id);
                }

                await ValidateDuplication(request, productRepository, cancellationToken);

                _mapper.Map(request, product);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Product {ProductId} updated successfully.", request.Id);
                return _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error while updating product with id: {ProductId}", request.Id);
                throw;
            }
        }
        private static async Task ValidateDuplication(UpdateProductCommand request, IGenericRepository<Product, Guid> productRepository, CancellationToken cancellationToken)
        {
            if (await productRepository.AnyAsync(p => p.Name == request.Dto.Name && p.Id != request.Id, cancellationToken))
                throw new AlreadyExistsException<Product>($"Product with this Name : {request.Dto.Name} already exists");

            if (await productRepository.AnyAsync(p => p.SKU == request.Dto.SKU && p.Id != request.Id, cancellationToken))
                throw new AlreadyExistsException<Product>($"Product with SKU : {request.Dto.SKU} :already exists");
        }
    }
}
