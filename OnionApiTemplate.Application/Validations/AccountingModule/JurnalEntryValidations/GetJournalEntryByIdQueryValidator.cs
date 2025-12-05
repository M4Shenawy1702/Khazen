using Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Queries.GetById;

namespace Khazen.Application.Validations.AccountingModule.JurnalEntryValidations
{
    public class GetJournalEntryByIdQueryValidator : AbstractValidator<GetJournalEntryByIdQuery>
    {
        public GetJournalEntryByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Journal Entry Id is required.");
        }
    }
}
