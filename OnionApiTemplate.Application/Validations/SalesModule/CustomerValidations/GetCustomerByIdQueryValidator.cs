using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Queries.GetById;

namespace Khazen.Application.Validators.SalesModule.Customer
{
    public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
    {
        public GetCustomerByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Customer Id is required.");
        }
    }
}
