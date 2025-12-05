using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Update;

namespace Khazen.Application.Validations.SalesModule.SalesInvoiceValidations
{
    public class UpdateSalesInvoiceCommandValidator : AbstractValidator<UpdateSalesInvoiceCommand>
    {
        public UpdateSalesInvoiceCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Dto.InvoiceDate)
                .NotEmpty().WithMessage("Invoice Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Invoice Date cannot be in the future.");
        }
    }
}
