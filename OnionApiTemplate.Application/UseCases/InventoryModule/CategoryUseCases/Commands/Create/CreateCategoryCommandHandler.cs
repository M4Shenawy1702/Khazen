using Khazen.Application.DOTs.InventoryModule.CategoryDots;
using Khazen.Application.Specification.InventoryModule.CategorySpecifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Create
{
    internal class CreateCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateCategoryCommand> validator,
        ILogger<CreateCategoryCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateCategoryCommand> _validator = validator;
        private readonly ILogger<CreateCategoryCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to create Category: {CategoryName}", request.Dto.Name);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed for Category creation: {Errors}", string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            try
            {
                var user = await _userManager.FindByIdAsync(request.CurrentUserId);
                if (user is null)
                {
                    _logger.LogError("Audit Failure: User ID '{UserId}' not found during Category creation.", request.CurrentUserId);
                    throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
                }

                var repo = _unitOfWork.GetRepository<Category, Guid>();

                var existingCategory = await repo.GetAsync(new GetCategoryByNameSpecification(request.Dto.Name), cancellationToken, true);
                if (existingCategory is not null)
                {
                    _logger.LogWarning("Duplicate Category name detected: {CategoryName}", request.Dto.Name);
                    throw new AlreadyExistsException<Category>($"with Name '{request.Dto.Name}'");
                }

                var category = _mapper.Map<Category>(request.Dto);
                category.CreatedAt = DateTime.UtcNow;
                category.CreatedBy = user.Id;

                await repo.AddAsync(category, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Category '{CategoryName}' (ID: {CategoryId}) created successfully by User {UserId}",
                    category.Name, category.Id, user.Id);

                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex) when (ex is not DomainException && ex is not ApplicationException)
            {
                _logger.LogCritical(ex, "A database or system error occurred while creating category: {CategoryName}", request.Dto.Name);

                throw new ApplicationException("An unexpected error occurred while saving the category. Please try again later.");
            }
        }
    }
}
