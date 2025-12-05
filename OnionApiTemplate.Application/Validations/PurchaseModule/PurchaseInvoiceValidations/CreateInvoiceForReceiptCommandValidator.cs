using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Create;

namespace Khazen.Application.Validators.PurchaseModule.PurchaseInvoiceValidators
{
    public class CreateInvoiceForReceiptCommandValidator
        : AbstractValidator<CreateInvoiceForReceiptCommand>
    {
        public CreateInvoiceForReceiptCommandValidator()
        {
            RuleFor(x => x.Dto)
                .NotNull().WithMessage("Invoice data is required.");

            RuleFor(x => x.Dto.PurchaseReceiptId)
                .NotEmpty().WithMessage("Purchase receipt ID is required.");

            RuleFor(x => x.Dto.InvoiceNumber)
                .NotEmpty().WithMessage("Invoice number is required.")
                .MaximumLength(50).WithMessage("Invoice number cannot exceed 50 characters.");

            RuleForEach(x => x.Dto.Items)
                .SetValidator(new CreatePurchaseInvoiceItemDtoValidator());

            RuleFor(x => x.Dto.InvoiceDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Invoice date cannot be in the future.");

            RuleFor(x => x.Dto.Notes)
            .MaximumLength(300);

            RuleFor(x => x.Dto.Items)
                .NotNull().WithMessage("At least one invoice item is required.")
                .Must(items => items.Count > 0).WithMessage("At least one invoice item is required.");
        }
    }

    public class CreatePurchaseInvoiceItemDtoValidator
        : AbstractValidator<CreatePurchaseInvoiceItemDto>
    {
        public CreatePurchaseInvoiceItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required.");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Unit price must be greater than zero.");

            RuleFor(x => x.AdditionalCharges)
                .GreaterThanOrEqualTo(0).WithMessage("Additional charges cannot be negative.");

            RuleFor(x => x.Notes)
                .MaximumLength(200).WithMessage("Notes cannot exceed 200 characters.");
        }
    }
}
