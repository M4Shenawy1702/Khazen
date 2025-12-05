using Khazen.Application.BaseSpecifications.InventoryModule.CategorySpecifications;
using Khazen.Application.DOTs.InventoryModule.CategoryDots;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Queries.GetById
{
    internal class GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetCategoryByIdQueryHandler> logger)
        : IRequestHandler<GetCategoryByIdQuery, CategoryDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetCategoryByIdQueryHandler> _logger = logger;

        public async Task<CategoryDetailsDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting GetCategoryByIdQueryHandler for Category Name: {CategoryId}", request.Id);
            var repo = _unitOfWork.GetRepository<Category, Guid>();
            var category = await repo.GetAsync(new GetCategoryByIdSpecification(request.Id), cancellationToken, true);
            if (category is null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found.", request.Id);
                throw new NotFoundException<Category>(request.Id);
            }

            _logger.LogInformation("Category with ID {CategoryId} retrieved successfully.", request.Id);
            return _mapper.Map<CategoryDetailsDto>(category);
        }
    }
}
