using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Queries.GetAll;

namespace Khazen.Application.Validations.HRModule.PerformanceReviewValidations
{
    public class GetAllPerformanceReviewsQueryValidation
        : AbstractValidator<GetAllPerformanceReviewsQuery>
    {
        public GetAllPerformanceReviewsQueryValidation()
        {
            RuleFor(x => x.QueryParameters.To)
                .GreaterThanOrEqualTo(x => x.QueryParameters.From)
                .WithMessage("To date must be greater than or equal to From date.");
            RuleFor(x => x.QueryParameters.PageIndex)
                .GreaterThan(0)
                .WithMessage("Page index must be greater than 0.");
            RuleFor(x => x.QueryParameters.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("Page size must be between 1 and 50.");
        }
    }
}
