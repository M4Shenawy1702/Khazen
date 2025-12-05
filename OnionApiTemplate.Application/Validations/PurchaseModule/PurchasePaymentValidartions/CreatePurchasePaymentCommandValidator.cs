using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Commands.Create;

namespace Khazen.Application.Validations.PurchaseModule.PurchasePaymentValidartions
{
    public class CreatePurchasePaymentCommandValidator : AbstractValidator<CreatePurchasePaymentCommand>
    {
        public CreatePurchasePaymentCommandValidator()
        {
            RuleFor(x => x.Dto.PurchaseInvoiceId)
                .NotEmpty().WithMessage("PurchaseInvoiceId is required.");

            RuleFor(x => x.Dto.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Dto.Method)
                .IsInEnum().WithMessage("Invalid payment method.");
        }
    }
}
