using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Update;

namespace Khazen.Application.Validations.InventoryModule.CategoryValidations
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Dto.Name)
        .NotEmpty().WithMessage("Category name is required.")
        .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");
            RuleFor(x => x.Dto.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        }
    }
}
