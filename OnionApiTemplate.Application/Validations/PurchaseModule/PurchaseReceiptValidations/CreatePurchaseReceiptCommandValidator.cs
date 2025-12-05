using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Create;

namespace Khazen.Application.Validations.PurchaseModule.PurchaseReceiptValidations
{
    public class CreatePurchaseReceiptCommandValidator : AbstractValidator<CreatePurchaseReceiptCommand>
    {
        public CreatePurchaseReceiptCommandValidator()
        {
            RuleFor(x => x.Dto).NotNull().WithMessage("Receipt data is required.");

            RuleFor(x => x.Dto.PurchaseOrderId)
                .NotEmpty().WithMessage("Purchase order ID is required.");

            RuleFor(x => x.Dto.ReceivedByEmployeeId)
                .NotEmpty().WithMessage("Received by employee ID is required.");

            RuleFor(x => x.Dto.WarehouseId)
                .NotEmpty().WithMessage("Warehouse ID is required.");

            RuleFor(x => x.Dto.Items)
                .NotNull().WithMessage("Items list is required.")
                .Must(items => items.Count > 0).WithMessage("At least one item must be provided.");

            RuleForEach(x => x.Dto.Items)
                .SetValidator(new CreatePurchaseReceiptItemDtoValidator());
        }
    }

    public class CreatePurchaseReceiptItemDtoValidator : AbstractValidator<CreatePurchaseReceiptItemDto>
    {
        public CreatePurchaseReceiptItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required.");

            RuleFor(x => x.ReceivedQuantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        }
    }
}
