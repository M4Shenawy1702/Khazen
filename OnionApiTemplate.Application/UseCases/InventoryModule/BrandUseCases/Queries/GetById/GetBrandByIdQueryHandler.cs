using Khazen.Application.BaseSpecifications.InventoryModule.BrandSpecifications;
using Khazen.Application.DOTs.InventoryModule.BrandDtos;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.BrandUseCases.Queries.GetById
{
    internal class GetBrandByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetBrandByIdQueryHandler> logger
    ) : IRequestHandler<GetBrandByIdQuery, BrandDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetBrandByIdQueryHandler> _logger = logger;

        public async Task<BrandDetailsDto> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting GetBrandByIdQueryHandler for Brand ID: {BrandId}", request.Id);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (request.Id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid brand ID received.");
                    throw new BadRequestException("Invalid brand ID.");
                }

                var repo = _unitOfWork.GetRepository<Brand, Guid>();

                var brand = await repo.GetAsync(
                    new GetBrandByIdSpecification(request.Id),
                    cancellationToken,
                    asNoTracking: true
                );

                if (brand == null)
                {
                    _logger.LogWarning("Brand with ID {BrandId} was not found.", request.Id);
                    throw new NotFoundException<Brand>(request.Id);
                }

                _logger.LogInformation(
                    "Brand '{BrandName}' (ID: {BrandId}) retrieved successfully.",
                    brand.Name,
                    brand.Id
                );

                return _mapper.Map<BrandDetailsDto>(brand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetBrandByIdQueryHandler.");
                throw new BadRequestException("An error occurred while retrieving the brand.");
            }
        }
    }
}
