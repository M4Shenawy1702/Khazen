using Khazen.Application.BaseSpecifications.InventoryModule.BrandSpecifications;
using Khazen.Application.DOTs.InventoryModule.BrandDtos;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.BrandUseCases.Queries.GetAll
{
    internal class GetAllBrandsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllBrandsQueryHandler> logger
    ) : IRequestHandler<GetAllBrandsQuery, IEnumerable<BrandDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllBrandsQueryHandler> _logger = logger;

        public async Task<IEnumerable<BrandDto>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting GetAllBrandsQueryHandler...");

            try
            {
                var repo = _unitOfWork.GetRepository<Brand, Guid>();

                var brands = await repo.GetAllAsync(
                    new GetAllBrandsSpecification(),
                    cancellationToken,
                    asNoTracking: true
                );

                _logger.LogInformation("Retrieved {Count} brand(s) successfully.", brands.Count());

                return _mapper.Map<IEnumerable<BrandDto>>(brands);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in GetAllBrandsQueryHandler.");
                throw new BadRequestException("An error occurred while retrieving brands.");
            }
        }
    }
}
