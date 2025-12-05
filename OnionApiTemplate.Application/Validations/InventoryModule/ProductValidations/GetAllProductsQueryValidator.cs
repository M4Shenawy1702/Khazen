using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Queries.GetAll;

namespace Khazen.Application.Validations.InventoryModule.ProductValidations
{
    public class GetAllProductsQueryValidator : AbstractValidator<GetAllProductsQuery>
    {
        public GetAllProductsQueryValidator()
        {
            RuleFor(x => x.queryParameters)
                .NotNull()
                .WithMessage("Query parameters must be provided.");

            RuleFor(x => x.queryParameters.PageIndex)
                .GreaterThan(0)
                .WithMessage("Page index must be greater than zero.");

            RuleFor(x => x.queryParameters.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("Page size must be between 1 and 50.");

            RuleFor(x => x.queryParameters.ProductName)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.queryParameters.ProductName))
                .WithMessage("Product name cannot exceed 100 characters.");

            RuleFor(x => x.queryParameters.ProductSKU)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.queryParameters.ProductSKU))
                .WithMessage("Product SKU cannot exceed 50 characters.");

            RuleFor(x => x.queryParameters.ProductId)
                .Must(id => id == null || id != Guid.Empty)
                .WithMessage("Product ID, if provided, must be a valid GUID.");
        }
    }
}
