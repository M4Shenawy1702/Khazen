using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Update;

namespace Khazen.Application.Validations.PurchaseModule.PurchaseOrderValidations
{
    public class UpdatePurchaseOrderCommandValidator : AbstractValidator<UpdatePurchaseOrderCommand>
    {
        public UpdatePurchaseOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("PurchaseOrder Id is required.")
                .NotEqual(Guid.Empty).WithMessage("PurchaseOrder Id cannot be empty GUID.");

            RuleFor(x => x.Dto)
                .NotNull().WithMessage("UpdatePurchaseOrderDto is required.");

            RuleFor(x => x.Dto.SupplierId)
                .NotEmpty().WithMessage("Supplier is required.");

            RuleFor(x => x.Dto.Items)
                .NotNull().WithMessage("Items are required.")
                .Must(items => items.Count != 0).WithMessage("At least one item must be provided.");
        }
    }
}
