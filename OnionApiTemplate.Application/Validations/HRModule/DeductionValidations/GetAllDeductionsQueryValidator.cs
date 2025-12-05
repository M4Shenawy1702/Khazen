using Khazen.Application.UseCases.HRModule.DeductionUseCases.Queries.GetAll;

namespace Khazen.Application.Validations.HRModule.DeductionValidations
{
    public class GetAllDeductionsQueryValidator : AbstractValidator<GetAllDeductionsQuery>
    {
        public GetAllDeductionsQueryValidator()
        {
            RuleFor(x => x.QueryParameters.PageIndex)
                .GreaterThanOrEqualTo(1)
                .WithMessage("PageIndex must be greater than or equal to 1.");

            RuleFor(x => x.QueryParameters.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("PageSize must be between 1 and 100.");

            RuleFor(x => x.QueryParameters.From)
                .LessThanOrEqualTo(x => x.QueryParameters.To.Value)
                .When(x => x.QueryParameters.From.HasValue && x.QueryParameters.To.HasValue)
                .WithMessage("StartDate cannot be later than EndDate.");
        }
    }
}
