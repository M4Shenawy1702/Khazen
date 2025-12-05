using Khazen.Application.BaseSpecifications.InventoryModule.ProductSpecifications;
using Khazen.Application.Common;
using Khazen.Application.DOTs.InventoryModule.ProductDtos;
using Khazen.Domain.Entities.InventoryModule;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Queries.GetAll
{
    internal class GetAllProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<GetAllProductsQuery> validator, ILogger<GetAllProductsQueryHandler> logger) : IRequestHandler<GetAllProductsQuery, PaginatedResult<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetAllProductsQuery> _validator = validator;
        private readonly ILogger<GetAllProductsQueryHandler> _logger = logger;

        public async Task<PaginatedResult<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("GetAllProductsQueryHandler started.");
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogError(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));
                    throw new ValidationException(validationResult.Errors);
                }

                var repo = _unitOfWork.GetRepository<Product, Guid>();
                var products = await repo.GetAllAsync(new GetAllProductsSpecification(request.queryParameters), cancellationToken, true);
                var productDtos = _mapper.Map<List<ProductDto>>(products);

                var count = await repo.GetCountAsync(new GetAllProductsCountSpecification(request.queryParameters), cancellationToken);

                _logger.LogInformation("GetAllProductsQueryHandler completed. Total records found: {Count}", count);

                return new PaginatedResult<ProductDto>(request.queryParameters.PageIndex, request.queryParameters.PageSize, count, productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error occurred while fetching products.");
                throw;
            }
        }
    }
}
