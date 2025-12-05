using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Update;
using Microsoft.AspNetCore.Http;

namespace Khazen.Application.Validations.InventoryModule.ProductValidations
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Dto.PurchasePrice)
                .GreaterThan(0).WithMessage("Purchase price must be greater than 0.");

            RuleFor(x => x.Dto.SellingPrice)
                .GreaterThan(0).WithMessage("Selling price must be greater than 0.")
                .GreaterThan(x => x.Dto.PurchasePrice).WithMessage("Selling price must be higher than purchase price.");

            RuleFor(x => x.Dto.QuantityInStock)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity in stock cannot be negative.");

            RuleFor(x => x.Dto.MinimumStock)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum stock cannot be negative.");

            RuleFor(x => x.Dto.SKU)
                .NotEmpty().WithMessage("SKU is required.")
                .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.");

            RuleFor(x => x.Dto.BrandId)
                .NotEmpty().WithMessage("Brand is required.");

            RuleFor(x => x.Dto.CategoryId)
                .NotEmpty().WithMessage("Category is required.");

            RuleFor(x => x.Dto.Image)
                .NotNull().WithMessage("Product image is required.")
                .Must(file => file.Length > 0).WithMessage("Invalid image file.")
                .Must(file => IsValidImage(file)).WithMessage("Only PNG, JPG, and JPEG formats are allowed.");
        }

        private bool IsValidImage(IFormFile file)
        {
            if (file == null) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return allowedExtensions.Contains(extension);
        }
    }
}

