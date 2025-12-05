using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Create;

namespace Khazen.Application.Validations.SalesModule.SalesPaymentValidations
{
    public class CreateSalesOrderPaymentCommandValidator : AbstractValidator<CreateSalesInvoicePaymentCommand>
    {
        public CreateSalesOrderPaymentCommandValidator()
        {
            RuleFor(x => x.Dto.SalesInvoiceId)
                .NotEmpty().WithMessage("Sales invoice is required.");

            RuleFor(x => x.Dto.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Dto.Method)
                .IsInEnum().WithMessage("Invalid payment method.");
        }
    }
}
