using Khazen.Application.DOTs.InventoryModule.CategoryDots;
using Khazen.Application.Specification.InventoryModule.CategorySpecifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Create
{
    internal class CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateCategoryCommand> validator, ILogger<CreateCategoryCommandHandler> logger)
        : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateCategoryCommand> _validator = validator;
        private readonly ILogger<CreateCategoryCommandHandler> _logger = logger;

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting CreateCategoryCommandHandler for Category Name: {CategoryName}", request.Dto.Name);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed: {@Errors}", validationResult.Errors);
                throw new BadRequestException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var repo = _unitOfWork.GetRepository<Category, Guid>();

            if (await repo.GetAsync(new GetCategoryByNameSpecification(request.Dto.Name), cancellationToken, true) is not null)
            {
                _logger.LogWarning("Attempted to create duplicate category with name '{CategoryName}'", request.Dto.Name);
                throw new AlreadyExistsException<Category>($"with Name '{request.Dto.Name}'");
            }

            var category = _mapper.Map<Category>(request.Dto);

            await repo.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = _mapper.Map<CategoryDto>(category);
            var now = DateTime.UtcNow;
            _logger.LogInformation(
                "Category '{CategoryName}' with ID {CategoryId} created successfully at {Time}.",
                category.Name, category.Id, now);

            return result;
        }
    }
}
