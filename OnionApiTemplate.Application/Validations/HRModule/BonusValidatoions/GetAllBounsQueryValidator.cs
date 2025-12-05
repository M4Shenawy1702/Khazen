using Khazen.Application.UseCases.HRModule.BonusUseCases.Queries.GetAll;

namespace Khazen.Application.Validations.HRModule.BonusValidatoions
{
    public class GetAllBounsQueryValidator : AbstractValidator<GetAllBounsQuery>
    {
        public GetAllBounsQueryValidator()
        {
            RuleFor(x => x.QueryParameters)
                .NotNull()
                .WithMessage("Query parameters are required.");

            RuleFor(x => x.QueryParameters.PageIndex)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page index must be greater than or equal to 1.");

            RuleFor(x => x.QueryParameters.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.QueryParameters.From)
                .LessThanOrEqualTo(x => x.QueryParameters.To)
                .When(x => x.QueryParameters.From.HasValue && x.QueryParameters.From.HasValue)
                .WithMessage("Start date cannot be after end date.");

            RuleFor(x => x.QueryParameters.EmployeeId)
                .Must(id => id == null || id != Guid.Empty)
                .WithMessage("Invalid Employee ID.");

            RuleFor(x => x.QueryParameters.To)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .When(x => x.QueryParameters.To.HasValue)
                .WithMessage("End date cannot be in the future.");
        }
    }
}
