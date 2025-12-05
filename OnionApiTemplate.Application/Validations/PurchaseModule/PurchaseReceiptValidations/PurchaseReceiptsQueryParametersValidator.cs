using Khazen.Application.Common.QueryParameters;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Queries.GetAll;

namespace Khazen.Application.Validations.PurchaseModule.PurchaseReceiptValidations
{
    public class PurchaseReceiptsQueryParametersValidator
        : AbstractValidator<PurchaseReceiptsQueryParameters>
    {
        public PurchaseReceiptsQueryParametersValidator()
        {
            RuleFor(x => x.StatDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("Start date must be less than or equal to end date.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StatDate)
                .WithMessage("End date must be greater than or equal to start date.");

            RuleFor(x => x.PageIndex)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100.");
        }
    }

    public class GetAllPurchaseReceiptsQueryValidator
        : AbstractValidator<GetAllPurchaseReceiptsQuery>
    {
        public GetAllPurchaseReceiptsQueryValidator()
        {
            RuleFor(x => x.QueryParameters)
                .NotNull()
                .SetValidator(new PurchaseReceiptsQueryParametersValidator());
        }
    }
}
