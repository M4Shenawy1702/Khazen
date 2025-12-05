using Khazen.Application.UseCases.HRModule.AdvanceUseCases.Queries.GetAll;

namespace Khazen.Application.Validations.HRModule.AdvanceValidations
{
    public class GetAllAdvanceQueryValidator : AbstractValidator<GetAllAdvanceQuery>
    {
        public GetAllAdvanceQueryValidator()
        {
            RuleFor(x => x.QueryParameters)
                .NotNull()
                .WithMessage("Query parameters are required.");

            When(x => x.QueryParameters != null, () =>
            {
                RuleFor(x => x.QueryParameters.PageIndex)
                    .GreaterThanOrEqualTo(1)
                    .WithMessage("Page index must be greater than or equal to 1.");

                RuleFor(x => x.QueryParameters.PageSize)
                    .InclusiveBetween(1, 100)
                    .WithMessage("Page size must be between 1 and 100.");

                RuleFor(x => x.QueryParameters.From)
                    .LessThanOrEqualTo(x => x.QueryParameters.To)
                    .When(x => x.QueryParameters.From.HasValue && x.QueryParameters.To.HasValue)
                    .WithMessage("Start date must be earlier than or equal to end date.");

                RuleFor(x => x.QueryParameters.EmployeeId)
                    .Must(id => id == null || id != Guid.Empty)
                    .WithMessage("Employee ID, if provided, cannot be empty.");
            });
        }
    }
}
