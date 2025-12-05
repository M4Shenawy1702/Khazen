using Khazen.Application.Common.Interfaces;
using Khazen.Application.DOTs.AccountingModule.JournalEntryDots;
using Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Commands.Create
{
    public class CreateJournalEntryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateJournalEntryCommand> validator, INumberSequenceService numberSequenceService
        , ILogger<CreateJournalEntryCommandHandler> logger)
                : IRequestHandler<CreateJournalEntryCommand, JournalEntryDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateJournalEntryCommand> _validator = validator;
        private readonly INumberSequenceService _numberSequenceService = numberSequenceService;
        private readonly ILogger<CreateJournalEntryCommandHandler> _logger = logger;

        public async Task<JournalEntryDto> Handle(CreateJournalEntryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting CreateJournalEntryCommandHandler");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogError("Validation failed: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                if (Math.Round(request.Dto.Lines.Sum(l => l.Debit), 2) != Math.Round(request.Dto.Lines.Sum(l => l.Credit), 2))
                {
                    _logger.LogWarning("Debits and Credits must be equal.");
                    throw new BadRequestException("Debits and Credits must be equal.");
                }

                var journalEntryRepo = _unitOfWork.GetRepository<JournalEntry, Guid>();
                var accountRepo = _unitOfWork.GetRepository<Account, Guid>();

                var journalNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken);

                var accountIds = request.Dto.Lines.Select(l => l.AccountId).ToHashSet();
                var existingIds = await accountRepo.GetAllAsync(new GetBatchOfAccountsByIdSpec(accountIds), cancellationToken);
                var existingIdsList = existingIds.Select(a => a.Id).ToList();

                if (existingIdsList.Count != accountIds.Count)
                {
                    _logger.LogWarning("One or more accounts not found.");
                    throw new NotFoundException<Account>("One or more accounts not found.");
                }

                var now = DateTime.UtcNow;

                var entry = new JournalEntry()
                {
                    CreatedAt = now,
                    JournalEntryNumber = journalNumber,
                    Description = request.Dto.Description,
                    EntryDate = request.Dto.EntryDate,
                    TransactionType = request.Dto.TransactionType,

                    Lines = request.Dto.Lines.Select(l => new JournalEntryLine()
                    {
                        Debit = l.Debit,
                        Credit = l.Credit,
                        AccountId = l.AccountId,
                        Description = l.Description,
                        CreatedAt = now
                    }).ToList()
                };
                UpdateBalances(accountRepo, existingIds, entry);

                await journalEntryRepo.AddAsync(entry, cancellationToken);

                _logger.LogInformation("Journal entry {JournalEntryId} created successfully.", entry.Id);

                var result = _mapper.Map<JournalEntryDto>(entry);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating journal entry");
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        private static void UpdateBalances(IGenericRepository<Account, Guid> accountRepo, IEnumerable<Account> existingIds, JournalEntry entry)
        {
            foreach (var line in entry.Lines)
            {
                var account = existingIds.First(a => a.Id == line.AccountId);

                switch (account.AccountType)
                {
                    case AccountType.Asset:
                    case AccountType.Expense:
                        account.Balance += (line.Debit - line.Credit);
                        break;

                    case AccountType.Liability:
                    case AccountType.Revenue:
                    case AccountType.Equity:
                        account.Balance += (line.Credit - line.Debit);
                        break;
                }

                account.ModifiedAt = DateTime.UtcNow;
                accountRepo.Update(account);
            }
        }
    }

}
