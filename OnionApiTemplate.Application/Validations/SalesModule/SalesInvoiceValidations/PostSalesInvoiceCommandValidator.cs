using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Post;

namespace Khazen.Application.Validations.SalesModule.SalesInvoiceValidations
{
    public class PostSalesInvoiceCommandValidator : AbstractValidator<PostSalesInvoiceCommand>
    {
        public PostSalesInvoiceCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("SalesOrderId is required.");
            RuleFor(x => x.PostedBy).NotEmpty().WithMessage("PostedBy is required.");
        }
    }
}
