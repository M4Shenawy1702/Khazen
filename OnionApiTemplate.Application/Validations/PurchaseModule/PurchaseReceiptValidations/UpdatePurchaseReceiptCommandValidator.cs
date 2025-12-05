using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Update;

namespace Khazen.Application.Validations.PurchaseModule.PurchaseReceiptValidations
{
    public class UpdatePurchaseReceiptCommandValidator
        : AbstractValidator<UpdatePurchaseReceiptCommand>
    {
        public UpdatePurchaseReceiptCommandValidator()
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .WithMessage("Update data must be provided.");

            RuleFor(x => x.Dto.RowVersion)
                .NotNull()
                .WithMessage("RowVersion is required for concurrency control.")
                .Must(rv => rv.Length > 0)
                .WithMessage("RowVersion cannot be empty.");

            RuleFor(x => x.Dto.ReceiptDate)
                .NotEmpty()
                .WithMessage("Receipt date must be provided.");

            RuleFor(x => x.Dto.Notes)
                .MaximumLength(500)
                .WithMessage("Notes cannot exceed 500 characters.");

            RuleFor(x => x.Dto.Items)
                .NotNull()
                .WithMessage("Receipt must have at least one item.")
                .Must(items => items != null && items.Count > 0)
                .WithMessage("Receipt must have at least one item.")
                .ForEach(item =>
                {
                    item.SetValidator(new UpdatePurchaseReceiptItemDtoValidator());
                });
        }
    }

    public class UpdatePurchaseReceiptItemDtoValidator : AbstractValidator<UpdatePurchaseReceiptItemDto>
    {
        public UpdatePurchaseReceiptItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Received quantity must be greater than zero.");
        }
    }
}
