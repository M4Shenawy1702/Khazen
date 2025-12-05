using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Queries.GetAll;

namespace Khazen.Application.Validations.PurchaseModule.PurchaseOrderValidations
{
    public class GetAllPurchaseOrdersQueryValidator : AbstractValidator<GetAllPurchaseOrdersQuery>
    {
        public GetAllPurchaseOrdersQueryValidator()
        {
            RuleFor(x => x.QueryParameters.PageIndex)
                .GreaterThan(0).WithMessage("PageIndex must be greater than 0.");

            RuleFor(x => x.QueryParameters.PageSize)
                .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100.");
        }
    }
}
