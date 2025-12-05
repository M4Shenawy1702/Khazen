using Khazen.Application.BaseSpecifications.InventoryModule.CategorySpecifications;
using Khazen.Application.DOTs.InventoryModule.CategoryDots;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Queries.GetAll;
using Khazen.Domain.Entities.InventoryModule;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.CategoryUseCases
{
    internal class GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllCategoriesQueryHandler> logger)
        : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllCategoriesQueryHandler> _logger = logger;

        public async Task<IEnumerable<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting GetAllCategoriesQueryHandler ... ");

            var repo = _unitOfWork.GetRepository<Category, Guid>();
            var categories = await repo.GetAllAsync(new GetAllCategoriesSpecification(), cancellationToken, true);

            _logger.LogInformation("Fetched {Count} categories successfully.", categories.Count());

            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }
    }
}
