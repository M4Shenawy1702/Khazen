using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Create;

namespace Khazen.Application.Validations.SalesModule.SalesInvoiceValidations
{
    public class CreateSalesInvoiceCommandValidator : AbstractValidator<CreateSalesInvoiceCommand>
    {
        public CreateSalesInvoiceCommandValidator()
        {
            RuleFor(x => x.Dto.SalesOrderId)
                .NotEmpty()
                .WithMessage("SalesOrderId is required.");

            RuleFor(x => x.Dto.CustomerId)
                .NotEmpty()
                .WithMessage("CustomerId is required.");

            RuleFor(x => x.Dto.InvoiceDate)
                .NotEmpty()
                .WithMessage("Invoice date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Invoice date cannot be in the future.");
        }
    }
}
