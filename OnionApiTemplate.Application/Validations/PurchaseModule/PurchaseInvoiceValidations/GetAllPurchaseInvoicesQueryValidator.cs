using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Queries.GetAll;

namespace Khazen.Application.Validators.PurchaseModule.PurchaseInvoiceValidators
{
    public class GetAllPurchaseInvoicesQueryValidator
        : AbstractValidator<GetAllPurchaseInvoicesQuery>
    {
        public GetAllPurchaseInvoicesQueryValidator()
        {
            RuleFor(x => x.QueryParameters)
                .NotNull().WithMessage("Query parameters are required.");

            RuleFor(x => x.QueryParameters.PageIndex)
                .GreaterThan(0).WithMessage("Page number must be greater than zero.");

            RuleFor(x => x.QueryParameters.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than zero.")
                .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100.");

            //RuleFor(x => x.QueryParameters.PaymentStatus)
            //    .Empty().When(x => x.QueryParameters.PaymentStatus.HasValue);

        }
    }
}
