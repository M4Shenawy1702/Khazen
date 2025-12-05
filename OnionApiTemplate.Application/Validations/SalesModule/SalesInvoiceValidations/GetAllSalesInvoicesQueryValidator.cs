using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Queries.GetAll;

namespace Khazen.Application.Validations.SalesModule.SalesInvoiceValidations
{
    public class GetAllSalesInvoicesQueryValidator : AbstractValidator<GetAllSalesInvoicesQuery>
    {
        public GetAllSalesInvoicesQueryValidator()
        {
            RuleFor(x => x.QueryParameters.PageIndex)
                .GreaterThan(0)
                .WithMessage("PageIndex must be greater than 0.");

            RuleFor(x => x.QueryParameters.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(50)
                .WithMessage("PageSize must be between 1 and 50.");
        }
    }
}
