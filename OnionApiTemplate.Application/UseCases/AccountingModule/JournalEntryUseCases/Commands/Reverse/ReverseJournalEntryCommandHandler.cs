using Khazen.Application.Common.Interfaces;
using Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Commands.Delete
{
    public class ReverseJournalEntryCommandHandler(
    IUnitOfWork unitOfWork,
    INumberSequenceService numberSequenceService,
    IValidator<ReverseJournalEntryCommand> validator,
    ILogger<ReverseJournalEntryCommandHandler> logger,
    IJournalEntryService journalEntryService,
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<ReverseJournalEntryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INumberSequenceService _numberSequenceService = numberSequenceService;
        private readonly IValidator<ReverseJournalEntryCommand> _validator = validator;
        private readonly ILogger<ReverseJournalEntryCommandHandler> _logger = logger;
        private readonly IJournalEntryService _journalEntryService = journalEntryService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(ReverseJournalEntryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting ReverseJournalEntryCommandHandler");

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var user = await _userManager.FindByNameAsync(request.ReversedBy);
            if (user is null)
            {
                _logger.LogInformation("User not found. Username: {ReversedBy}", request.ReversedBy);
                throw new NotFoundException<ApplicationUser>(request.ReversedBy);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {


                var journalEntryRepo = _unitOfWork.GetRepository<JournalEntry, Guid>();
                var accountRepo = _unitOfWork.GetRepository<Account, Guid>();

                var entry = await journalEntryRepo.GetAsync(new GetJurnalEntryByIdWithIncludesSpecification(request.Id), cancellationToken);
                if (entry == null)
                {
                    _logger.LogError("Entry with Id : {EntryId} was not found", request.Id);
                    throw new NotFoundException<JournalEntry>(request.Id);
                }

                if (request.RowVersion == null || request.RowVersion.Length == 0)
                {
                    _logger.LogError("Missing concurrency token for Entry {EntryId}", request.Id);
                    throw new BadRequestException("The concurrency token (RowVersion) is required for this operation.");
                }

                entry.SetRowVersion(request.RowVersion);

                if (entry.IsReversed)
                {
                    _logger.LogError("Entry with Id : {EntryId} already reversed", request.Id);
                    throw new BadRequestException("This journal entry has already been reversed.");
                }
                if (entry.IsReversal)
                {
                    _logger.LogError("Entry with Id : {EntryId} is a reversal, cannot be reversed", request.Id);
                    throw new BadRequestException("This journal entry has already been reversed.");
                }

                if (entry.Lines.Count == 0)
                    throw new BadRequestException("Journal entry has no lines to reverse.");

                var reversedEntry = entry.CreateReversal(request.ReversedBy, $"Reversed JE for JE-Number : {entry.JournalEntryNumber}");

                reversedEntry.JournalEntryNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken);
                reversedEntry.CreatedAt = DateTime.UtcNow;

                var accountIds = reversedEntry.Lines.Select(l => l.AccountId).Distinct().ToHashSet();
                var accounts = await accountRepo.GetAllAsync(new GetBatchOfAccountsByIdSpec(accountIds), cancellationToken);

                _journalEntryService.ReverseUpdateBalances(accounts, reversedEntry);
                foreach (var account in accounts.Distinct())
                {
                    var updatedEntry = _unitOfWork.Context.Entry(account);

                    if (updatedEntry.State == EntityState.Detached)
                    {
                        _unitOfWork.Context.Attach(account);
                    }

                    updatedEntry.Property(nameof(account.Balance)).IsModified = true;
                    updatedEntry.Property(nameof(account.ModifiedAt)).IsModified = true;

                }
                journalEntryRepo.Update(entry);

                await journalEntryRepo.AddAsync(reversedEntry, cancellationToken);

                _logger.LogInformation(
                    "Reversed Journal Entry {OriginalEntryNumber} -> {ReversalNumber} | OriginalId: {OriginalEntryId}, ReversedId: {ReversedEntryId}, Date: {Date}",
                    entry.JournalEntryNumber, reversedEntry.JournalEntryNumber, entry.Id, reversedEntry.Id, DateTime.UtcNow);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return true;
            }
            catch (DBConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict detected for Journal Entry {Id}", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw new ConcurrencyException("The journal entry was modified by another user. Please refresh and try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reversing journal entry");
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }


    }
}
