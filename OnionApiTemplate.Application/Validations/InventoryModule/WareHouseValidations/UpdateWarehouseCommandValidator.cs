using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Update;

namespace Khazen.Application.Validations.InventoryModule.WarehouseValidations
{
    public class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommand>
    {
        public UpdateWarehouseCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Warehouse ID is required.");

            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Warehouse name is required.")
                .MaximumLength(200).WithMessage("Warehouse name must not exceed 200 characters.");

            RuleFor(x => x.Dto.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(500).WithMessage("Address must not exceed 500 characters.");

            RuleFor(x => x.Dto.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9\s\-]{8,15}$")
                .WithMessage("Phone number must be valid and contain only digits, spaces, or dashes.");

            RuleFor(x => x.Dto.WarehouseProducts)
             .Must(products => products.Select(p => p.ProductId).Distinct().Count() == products.Count)
             .WithMessage("Duplicate products are not allowed in warehouse inventory.");

            RuleForEach(x => x.Dto.WarehouseProducts)
                .ChildRules(product =>
                {
                    product.RuleFor(p => p.ProductId)
                        .NotEmpty().WithMessage("Product ID is required for each warehouse product.");

                    product.RuleFor(p => p.QuantityInStock)
                        .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");
                });
        }
    }
}
