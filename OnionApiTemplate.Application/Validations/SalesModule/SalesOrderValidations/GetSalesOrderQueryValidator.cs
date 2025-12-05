using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Queries.GetById;

namespace Khazen.Application.Validations.SalesModule.SalesOrderValidations
{
    public class GetSalesOrderQueryValidator : AbstractValidator<GetSalesOrderQuery>
    {
        public GetSalesOrderQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Order Id is required.");
        }
    }
}
