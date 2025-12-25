using Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetAll;

namespace Khazen.Application.Validations.HRModule.SalaryValidations
{
    public class SalariesQueryParametersValidator : AbstractValidator<GetAllSalariesQuery>
    {
        public SalariesQueryParametersValidator()
        {
            RuleFor(x => x.QueryParameters.PageIndex)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page index must start from 1.");

            RuleFor(x => x.QueryParameters.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.QueryParameters.From)
                .LessThanOrEqualTo(x => x.QueryParameters.To)
                .When(x => x.QueryParameters.From.HasValue && x.QueryParameters.To.HasValue)
                .WithMessage("'From' date cannot be after 'To' date.");

            RuleFor(x => x.QueryParameters.EmployeeId)
                .Must(id => id == null || id != Guid.Empty)
                .WithMessage("EmployeeId must be a valid GUID if provided.");

            RuleFor(x => x.QueryParameters.SalarySortOption)
                .IsInEnum()
                .WithMessage("Invalid sorting option for salaries.");
        }
    }
}
