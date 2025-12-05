using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Create;

namespace Khazen.Application.Validations.PurchaseModule.PurchaseOrderValidations
{
    public class CreatePurchaseOrderValidator : AbstractValidator<CreatePurchaseOrderCommand>
    {
        public CreatePurchaseOrderValidator()
        {
            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required.");

            RuleFor(x => x.Dto.SupplierId)
                .NotEmpty().WithMessage("SupplierId is required.");

            RuleFor(x => x.Dto.Items)
                .NotEmpty().WithMessage("At least one item is required.");

            RuleFor(x => x.Dto.DeliveryDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("DeliveryDate must be in the future.");

            RuleForEach(x => x.Dto.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.ProductId)
                        .NotEmpty().WithMessage("ProductId is required.");

                    item.RuleFor(i => i.Quantity)
                        .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
                        .LessThan(100000).WithMessage("Quantity is too large.");

                    item.RuleFor(i => i.ExpectedUnitPrice)
                        .GreaterThan(0).WithMessage("UnitPrice must be greater than 0.");
                });

            RuleFor(x => x.Dto.Notes)
                .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");
        }
    }
}
