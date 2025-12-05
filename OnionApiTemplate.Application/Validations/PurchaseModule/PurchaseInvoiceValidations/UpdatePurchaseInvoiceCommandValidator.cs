using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Update;

namespace Khazen.Application.Validations.PurchaseModule.PurchaseInvoiceValidations
{
    public class UpdatePurchaseInvoiceCommandValidator : AbstractValidator<UpdatePurchaseInvoiceCommand>
    {
        public UpdatePurchaseInvoiceCommandValidator()
        {
            RuleFor(x => x.Dto).NotNull()
                .WithMessage("Invoice data is required.");

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("InvoiceId is required.");

            RuleFor(x => x.Dto.InvoiceNumber)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Invoice number is required and must not exceed 50 characters.");

        }
    }
}
