using Khazen.Application.Common.Interfaces;
using Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Commands.Delete
{
    public class ReverseJournalEntryCommandHandler(
        IUnitOfWork unitOfWork,
        INumberSequenceService numberSequenceService,
        IValidator<ReverseJournalEntryCommand> validator,
        ILogger<ReverseJournalEntryCommandHandler> logger)
        : IRequestHandler<ReverseJournalEntryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INumberSequenceService _numberSequenceService = numberSequenceService;
        private readonly IValidator<ReverseJournalEntryCommand> _validator = validator;
        private readonly ILogger<ReverseJournalEntryCommandHandler> _logger = logger;

        public async Task<bool> Handle(ReverseJournalEntryCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                _logger.LogDebug("Starting ReverseJournalEntryCommandHandler");

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogError("Validation failed: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var journalEntryRepo = _unitOfWork.GetRepository<JournalEntry, Guid>();
                var accountRepo = _unitOfWork.GetRepository<Account, Guid>();

                var entry = await journalEntryRepo.GetAsync(new GetJurnalEntryByIdWithIncludesSpecification(request.Id), cancellationToken);
                if (entry == null)
                {
                    _logger.LogError("Entry with Id : {EntryId} was not found", request.Id);
                    throw new NotFoundException<JournalEntry>(request.Id);
                }

                if (entry.IsReversed)
                {
                    _logger.LogError("Entry with Id : {EntryId} already reversed", request.Id);
                    throw new BadRequestException("This journal entry has already been reversed.");
                }

                if (entry.Lines.Count == 0)
                    throw new BadRequestException("Journal entry has no lines to reverse.");

                var reversedNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken);
                var now = DateTime.UtcNow;

                var reversedEntry = new JournalEntry
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = now,
                    JournalEntryNumber = reversedNumber,
                    Description = $"Reversal of Journal Entry #{entry.JournalEntryNumber}",
                    EntryDate = now,
                    TransactionType = entry.TransactionType,
                    IsReversal = true,
                    ReversalOfJournalEntryId = entry.Id,
                    Lines = entry.Lines.Select(line => new JournalEntryLine
                    {
                        AccountId = line.AccountId,
                        Debit = line.Credit,
                        Credit = line.Debit,
                        Description = $"Reversal of Line {line.Id}",
                        CreatedAt = now
                    }).ToList()
                };
                await UpdateBalances(accountRepo, reversedEntry, cancellationToken);

                await journalEntryRepo.AddAsync(reversedEntry, cancellationToken);

                entry.IsReversed = true;
                entry.ReversedAt = now;

                journalEntryRepo.Update(entry);

                _logger.LogInformation(
                "Reversed Journal Entry {OriginalEntryNumber} -> {ReversalNumber} | OriginalId: {OriginalEntryId}, ReversedId: {ReversedEntryId}, Date: {Date}",
                entry.JournalEntryNumber, reversedEntry.JournalEntryNumber, entry.Id, reversedEntry.Id, now);



                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reversing journal entry");
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        private static async Task UpdateBalances(
            IGenericRepository<Account, Guid> accountRepo,
            JournalEntry reversedEntry,
            CancellationToken cancellationToken)
        {
            var accountIds = reversedEntry.Lines.Select(l => l.AccountId).Distinct().ToHashSet();
            var accounts = await accountRepo.GetAllAsync(new GetBatchOfAccountsByIdSpec(accountIds), cancellationToken);

            foreach (var line in reversedEntry.Lines)
            {
                var account = accounts.First(a => a.Id == line.AccountId);
                var amount = line.Debit - line.Credit;

                account.Balance += account.AccountType switch
                {
                    AccountType.Asset or AccountType.Expense => amount,
                    AccountType.Liability or AccountType.Revenue or AccountType.Equity => -amount,
                    _ => 0
                };

                account.ModifiedAt = DateTime.UtcNow;
            }

            accountRepo.UpdateRange(accounts);

        }

    }
}
