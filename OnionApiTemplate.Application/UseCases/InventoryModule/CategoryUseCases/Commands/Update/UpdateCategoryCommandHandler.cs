using Khazen.Application.BaseSpecifications.InventoryModule.CategorySpecifications;
using Khazen.Application.DOTs.InventoryModule.CategoryDots;
using Khazen.Application.Specification.InventoryModule.CategorySpecifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Update
{
    internal class UpdateCategoryCommandHandler(IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateCategoryCommand> validator,
        ILogger<UpdateCategoryCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<UpdateCategoryCommand, CategoryDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateCategoryCommand> _validator = validator;
        private readonly ILogger<UpdateCategoryCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<CategoryDetailsDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting UpdateCategoryCommandHandler for Category ID : {CategoryId}", request.Id);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed: {@Errors}", validationResult.Errors);
                throw new BadRequestException(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            var user = await _userManager.FindByIdAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogError("User with ID '{UserId}' not found", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>($"with ID '{request.CurrentUserId}'");
            }

            var repo = _unitOfWork.GetRepository<Category, Guid>();

            var nameCheckTask = repo.GetAsync(new GetCategoryByNameSpecification(request.Dto.Name), cancellationToken, true);
            var categoryTask = repo.GetAsync(new GetCategoryByIdSpecification(request.Id), cancellationToken);

            await Task.WhenAll(nameCheckTask, categoryTask);

            var existing = await nameCheckTask;
            if (existing is not null && existing.Id != request.Id)
            {
                _logger.LogWarning("Attempted to update category to a duplicate name '{CategoryName}'", request.Dto.Name);
                throw new AlreadyExistsException<Category>($"with Name '{request.Dto.Name}'");
            }

            var category = await categoryTask;
            if (category is null)
            {
                _logger.LogWarning("Attempted to update non-existing category with ID {CategoryId}.", request.Id);
                throw new NotFoundException<Category>(request.Id);
            }

            _mapper.Map(request.Dto, category);
            category.ModifiedBy = user.Id;
            category.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Category '{CategoryName}' with ID {CategoryId} updated successfully.", category.Name, category.Id);

            return _mapper.Map<CategoryDetailsDto>(category);
        }
    }
}
