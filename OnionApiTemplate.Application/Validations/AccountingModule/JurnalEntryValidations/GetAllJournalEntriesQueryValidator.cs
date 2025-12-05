using Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Queries.GetAll;

namespace Khazen.Application.Validations.AccountingModule.JurnalEntryValidations
{
    public class GetAllJournalEntriesQueryValidator : AbstractValidator<GetPaginatedJournalEntriesQuery>
    {
        public GetAllJournalEntriesQueryValidator()
        {
            RuleFor(x => x.QueryParameters)
                .NotNull().WithMessage("Query parameters are required.");

            RuleFor(x => x.QueryParameters.PageIndex)
                .GreaterThan(0).WithMessage("PageNumber must be greater than 0.");

            RuleFor(x => x.QueryParameters.PageSize)
                .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100.");
        }
    }
}
