using Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Commands.Create;

namespace Khazen.Application.Validations.AccountingModule.JurnalEntryValidations
{
    public class CreateJournalEntryCommandValidator : AbstractValidator<CreateJournalEntryCommand>
    {
        public CreateJournalEntryCommandValidator()
        {
            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(250).WithMessage("Description must not exceed 250 characters.");

            RuleFor(x => x.Dto.EntryDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Entry date cannot be in the future.");

            RuleFor(x => x.Dto.TransactionType)
                .IsInEnum().WithMessage("Invalid transaction type.");

            RuleFor(x => x.Dto.Lines)
                .NotNull().WithMessage("Lines are required.")
                .NotEmpty().WithMessage("At least one journal entry line is required.");

            RuleForEach(x => x.Dto.Lines).ChildRules(lines =>
            {
                lines.RuleFor(l => l.AccountId)
                    .NotEmpty().WithMessage("Account Id is required for each line.");

                lines.RuleFor(l => l.Debit)
                    .GreaterThanOrEqualTo(0).WithMessage("Debit must be 0 or greater.");

                lines.RuleFor(l => l.Credit)
                    .GreaterThanOrEqualTo(0).WithMessage("Credit must be 0 or greater.");
            });

            RuleFor(x => x)
                .Must(cmd => cmd.Dto.Lines.Sum(l => l.Debit) == cmd.Dto.Lines.Sum(l => l.Credit))
                .WithMessage("Total debit must equal total credit.");
        }
    }
}
