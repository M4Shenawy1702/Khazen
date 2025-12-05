using Khazen.Application.BaseSpecifications.InventoryModule.ProductSpecifications;
using Khazen.Application.DOTs.InventoryModule.ProductDtos;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Queries.GetById
{
    internal class GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetProductByIdQueryHandler> logger) : IRequestHandler<GetProductByIdQuery, ProductDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetProductByIdQueryHandler> _logger = logger;

        public async Task<ProductDetailsDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Starting GetProductByIdQueryHandler for Product ID: {ProductId}", request.Id);

                var productRepository = _unitOfWork.GetRepository<Product, Guid>();
                var product = await productRepository.GetAsync(new GetProductByIdspecifications(request.Id), cancellationToken, true);
                if (product is null)
                {
                    _logger.LogWarning("Product with ID: {ProductId} not found", request.Id);
                    throw new NotFoundException<Product>(request.Id);
                }

                _logger.LogInformation("GetProductByIdQueryHandler completed for Product ID: {ProductId}", request.Id);

                return _mapper.Map<ProductDetailsDto>(product);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error occurred while fetching product details.");
                throw;
            }
        }
    }
}
