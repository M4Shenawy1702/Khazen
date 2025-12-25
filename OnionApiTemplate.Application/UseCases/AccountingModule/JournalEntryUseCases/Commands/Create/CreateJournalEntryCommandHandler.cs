using Khazen.Application.Common.Interfaces;
using Khazen.Application.DOTs.AccountingModule.JournalEntryDots;
using Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Commands.Create
{

    public class CreateJournalEntryCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateJournalEntryCommand> validator,
        INumberSequenceService numberSequenceService,
        ILogger<CreateJournalEntryCommandHandler> logger,
        IJournalEntryService journalEntryService,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<CreateJournalEntryCommand, JournalEntryDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateJournalEntryCommand> _validator = validator;
        private readonly INumberSequenceService _numberSequenceService = numberSequenceService;
        private readonly ILogger<CreateJournalEntryCommandHandler> _logger = logger;
        private readonly IJournalEntryService _journalEntryService = journalEntryService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<JournalEntryDetailsDto> Handle(CreateJournalEntryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(" Starting CreateJournalEntryCommandHandler with request {@Request}", request);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug(" Transaction started");

            var user = await _userManager.FindByNameAsync(request.CreatedBy);
            if (user is null)
            {
                _logger.LogInformation("User not found. Username: {CreatedBy}", request.CreatedBy);
                throw new NotFoundException<ApplicationUser>(request.CreatedBy);
            }

            try
            {
                _logger.LogDebug(" Validating request");
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning(" Validation failed: {@Errors}", validationResult.Errors);
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var totalDebit = request.Dto.Lines.Sum(l => l.Debit);
                var totalCredit = request.Dto.Lines.Sum(l => l.Credit);
                _logger.LogDebug(" Debit={Debit}, Credit={Credit}", totalDebit, totalCredit);

                if (Math.Abs(totalDebit - totalCredit) > 0.0001m)
                {
                    _logger.LogWarning("Debit and Credit mismatch. Debit={Debit}, Credit={Credit}", totalDebit, totalCredit);
                    throw new BadRequestException("Debits and Credits must be equal.");
                }

                var journalEntryRepo = _unitOfWork.GetRepository<JournalEntry, Guid>();
                var accountRepo = _unitOfWork.GetRepository<Account, Guid>();

                _logger.LogDebug(" Fetching accounts for JE lines");
                var accountIds = request.Dto.Lines.Select(l => l.AccountId).ToHashSet();
                var accounts = await accountRepo.GetAllAsync(new GetBatchOfAccountsByIdSpec(accountIds), cancellationToken);
                var accountsDict = accounts.ToDictionary(a => a.Id, a => a);
                var accountsToUpdate = new List<Account>();

                if (accounts.Count() != accountIds.Count)
                {
                    _logger.LogWarning("Missing account(s). Requested={Requested}, Found={Found}", accountIds.Count, accounts.Count());
                    throw new NotFoundException<Account>("One or more accounts not found.");
                }

                var journalNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken);
                _logger.LogInformation("Generated Journal Entry Number: {JournalNumber}", journalNumber);

                var entry = new JournalEntry(
                    journalEntryNumber: journalNumber,
                    description: request.Dto.Description!,
                    entryDate: request.Dto.EntryDate,
                    transactionType: request.Dto.TransactionType,
                    relatedEntityType: null,
                    relatedEntityId: null,
                    createdBy: user.UserName!
                );

                _logger.LogDebug("Creating Journal Entry lines");
                foreach (var lineDto in request.Dto.Lines)
                {
                    var account = accountsDict[lineDto.AccountId];
                    accountsToUpdate.Add(account);

                    var line = new JournalEntryLine(
                        accountId: account.Id,
                        debit: lineDto.Debit,
                        credit: lineDto.Credit,
                        description: lineDto.Description
                    );

                    line.AttachToJournalEntry(entry.Id);
                    entry.Lines.Add(line);
                }

                entry.ValidateBalance();
                _logger.LogDebug("Journal entry lines balanced successfully");

                _logger.LogInformation("Updating balances for {Count} accounts", accounts.Count());
                _journalEntryService.UpdateBalances(accounts, entry);
                _logger.LogInformation("Marking only Balance and ModifiedAt for update on {Count} accounts", accountsToUpdate.Count);

                foreach (var account in accountsToUpdate.Distinct())
                {
                    var updatedEntry = _unitOfWork.Context.Entry(account);

                    updatedEntry.Property(nameof(account.Balance)).IsModified = true;
                    updatedEntry.Property(nameof(account.ModifiedAt)).IsModified = true;

                    updatedEntry.State = EntityState.Modified;
                }

                _logger.LogDebug("Saving Journal Entry to DB");
                await journalEntryRepo.AddAsync(entry, cancellationToken);

                var result = _mapper.Map<JournalEntryDetailsDto>(entry);
                _logger.LogInformation("Journal Entry {Id} created successfully", entry.Id);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Transaction committed");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating journal entry, rolling back transaction");
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }

}

