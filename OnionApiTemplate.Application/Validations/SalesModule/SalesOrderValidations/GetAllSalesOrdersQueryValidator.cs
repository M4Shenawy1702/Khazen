using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Queries.GetAll;

namespace Khazen.Application.Validations.SalesModule.SalesOrderValidations
{
    public class GetAllSalesOrdersQueryValidator : AbstractValidator<GetAllSalesOrdersQuery>
    {
        public GetAllSalesOrdersQueryValidator()
        {
            RuleFor(x => x.queryParameters.PageIndex)
                .GreaterThan(0).WithMessage("PageIndex must be greater than 0.");

            RuleFor(x => x.queryParameters.PageSize)
                .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
                .LessThanOrEqualTo(50).WithMessage("PageSize cannot exceed 50.");
        }
    }
}
