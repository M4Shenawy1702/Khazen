using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Queries.GetAll;

namespace Khazen.Application.Validators.SalesModule.Customer
{
    public class GetAllCustomersQueryValidator : AbstractValidator<GetAllCustomersQuery>
    {
        public GetAllCustomersQueryValidator()
        {
            RuleFor(x => x.QueryParameters.PageIndex)
                .GreaterThan(0).WithMessage("PageIndex must be greater than 0.");

            RuleFor(x => x.QueryParameters.PageSize)
                .InclusiveBetween(1, 50).WithMessage("PageSize must be between 1 and 50.");

            RuleFor(x => x.QueryParameters.CustomerType)
                .IsInEnum().WithMessage("Invalid customer type.");

        }
    }
}
