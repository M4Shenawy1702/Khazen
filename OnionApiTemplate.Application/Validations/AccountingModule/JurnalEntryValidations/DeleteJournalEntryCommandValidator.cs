using Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Commands.Delete;

namespace Khazen.Application.Validations.AccountingModule.JurnalEntryValidations
{
    public class DeleteJournalEntryCommandValidator : AbstractValidator<ReverseJournalEntryCommand>
    {
        public DeleteJournalEntryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Journal Entry Id is required.");
        }
    }
}
