using Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Queries.GetAll;

namespace Khazen.Application.Validations.PurchaseModule.PurchasePaymentValidartions
{
    public class GetAllPurchasePaymentsQueryValidator : AbstractValidator<GetAllPurchasePaymentsQuery>
    {
        public GetAllPurchasePaymentsQueryValidator()
        {
            RuleFor(x => x.QueryParameters.PageIndex)
                .GreaterThan(0)
                .WithMessage("PageNumber must be greater than 0.");

            RuleFor(x => x.QueryParameters.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1 and 100.");

            RuleFor(x => x.QueryParameters.Method)
                .IsInEnum()
                .When(x => x.QueryParameters.Method.HasValue)
                .WithMessage("Invalid PaymentMethod value.");
        }
    }
}
