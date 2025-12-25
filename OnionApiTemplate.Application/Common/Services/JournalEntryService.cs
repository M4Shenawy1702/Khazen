using Khazen.Application.Common.Interfaces;
using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Create;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services
{
    public class JournalEntryService : IJournalEntryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGetSystemValues _getSystemValues;
        private readonly INumberSequenceService _numberSequenceService;
        private readonly ILogger<JournalEntryService> _logger;

        public JournalEntryService(
            IUnitOfWork unitOfWork,
            IGetSystemValues getSystemValues,
            INumberSequenceService numberSequenceService,
            ILogger<JournalEntryService> logger)
        {
            _unitOfWork = unitOfWork;
            _getSystemValues = getSystemValues;
            _numberSequenceService = numberSequenceService;
            _logger = logger;
        }

        public async Task<JournalEntry> CreateSalaryJournalEntryAsync(
        Employee employee,
        Salary salary,
        string createdBy,
        CancellationToken cancellationToken = default)
        {
            var systemSettingsRepo = _unitOfWork.GetRepository<SystemSetting, int>();
            var systemSettings = await systemSettingsRepo.GetAllAsync(new GetAllSystemSettingsSpec(), cancellationToken);

            var salaryExpenseAccountValue = _getSystemValues.GetSettingValue(systemSettings, "SalaryExpenseAccountId");
            var cashAccountValue = _getSystemValues.GetSettingValue(systemSettings, "CashAccountId");

            if (!Guid.TryParse(salaryExpenseAccountValue, out var salaryExpenseAccountId))
            {
                _logger.LogError("Salary Expense Account ID is missing or invalid.");
                throw new ApplicationException("Salary Expense Account ID is missing or invalid.");
            }

            if (!Guid.TryParse(cashAccountValue, out var cashAccountId))
            {
                _logger.LogError("Cash Account ID is missing or invalid.");
                throw new ApplicationException("Cash Account ID is missing or invalid.");
            }

            var journalNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken);

            var journalEntry = new JournalEntry(
                journalEntryNumber: journalNumber,
                description: $"Salary for {employee.FirstName} {employee.LastName} - {salary.SalaryDate:MMMM yyyy}, created by '{createdBy}'",
                entryDate: DateTime.UtcNow,
                transactionType: TransactionType.SalaryExpense,
                relatedEntityType: RelatedEntityType.SalaryExpense,
                relatedEntityId: salary.Id,
                createdBy: createdBy
            );

            var debitLine = new JournalEntryLine(salaryExpenseAccountId, salary.NetSalary, 0m, "Salary Expense");
            debitLine.AttachToJournalEntry(journalEntry.Id);
            journalEntry.Lines.Add(debitLine);

            var creditLine = new JournalEntryLine(cashAccountId, 0m, salary.NetSalary, "Cash Payment");
            creditLine.AttachToJournalEntry(journalEntry.Id);
            journalEntry.Lines.Add(creditLine);

            journalEntry.ValidateBalance();

            _logger.LogInformation("Journal entry created for salary ID {SalaryId}", salary.Id);

            return journalEntry;
        }


        public async Task<JournalEntry> GeneratePurchaseJournalEntryAsync(
         PurchaseInvoice invoice,
         string CreatedBy,
         CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Generating Purchase Journal Entry for Invoice {InvoiceNo}", invoice.InvoiceNumber);
                cancellationToken.ThrowIfCancellationRequested();

                var systemSettingsRepo = _unitOfWork.GetRepository<SystemSetting, int>();
                var systemSettings = await systemSettingsRepo.GetAllAsync(new GetAllSystemSettingsSpec(), cancellationToken);

                var purchasesAccountId = Guid.Parse(_getSystemValues.GetSettingValue(systemSettings, "PurchasesAccountId"));
                var accountsPayableId = Guid.Parse(_getSystemValues.GetSettingValue(systemSettings, "AccountsPayableAccountId"));

                var journalNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken);

                var journal = new JournalEntry(
                    journalEntryNumber: journalNumber,
                    description: $"Purchase Invoice {invoice.InvoiceNumber}",
                    entryDate: DateTime.UtcNow,
                    transactionType: TransactionType.PurchaseInvoice,
                    relatedEntityType: RelatedEntityType.PurchaseInvoice,
                    relatedEntityId: invoice.Id,
                    createdBy: CreatedBy
                );

                var debitLine = new JournalEntryLine(purchasesAccountId, invoice.TotalAmount, 0, "Purchases");
                debitLine.AttachToJournalEntry(journal.Id);
                journal.Lines.Add(debitLine);

                var creditLine = new JournalEntryLine(accountsPayableId, 0, invoice.TotalAmount, "Accounts Payable");
                creditLine.AttachToJournalEntry(journal.Id);
                journal.Lines.Add(creditLine);

                journal.ValidateBalance();

                _logger.LogInformation(
                    "Purchase Journal Entry generated successfully for Invoice {InvoiceNo}.",
                    invoice.InvoiceNumber
                );

                return journal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate Purchase Journal Entry for Invoice {InvoiceNo}", invoice.InvoiceNumber);
                throw;
            }
        }


        public async Task<JournalEntry> GenerateReversalPurchaseInvoiceAsync(
            PurchaseInvoice invoice,
            string CreatedBy,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Generating Reversal Purchase Journal Entry for Invoice {InvoiceNo}", invoice.InvoiceNumber);
                cancellationToken.ThrowIfCancellationRequested();

                var systemSettingsRepo = _unitOfWork.GetRepository<SystemSetting, int>();
                var systemSettings = await systemSettingsRepo.GetAllAsync(new GetAllSystemSettingsSpec(), cancellationToken);

                var purchasesAccountId = Guid.Parse(_getSystemValues.GetSettingValue(systemSettings, "PurchasesAccountId"));
                var accountsPayableId = Guid.Parse(_getSystemValues.GetSettingValue(systemSettings, "AccountsPayableAccountId"));

                var journalNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken);

                var reversalEntry = invoice.JournalEntry!.CreateReversal(CreatedBy, $"Reversed Purchase invoice JE for Purchase invoice JE-Number : {invoice.JournalEntry.JournalEntryNumber}");

                var debitLine = new JournalEntryLine(accountsPayableId, invoice.TotalAmount, 0, "Accounts Payable Reversal");
                debitLine.AttachToJournalEntry(reversalEntry.Id);
                reversalEntry.Lines.Add(debitLine);

                var creditLine = new JournalEntryLine(purchasesAccountId, 0, invoice.TotalAmount, "Purchases Reversal");
                creditLine.AttachToJournalEntry(reversalEntry.Id);
                reversalEntry.Lines.Add(creditLine);

                reversalEntry.ValidateBalance();

                _logger.LogInformation(
                    "Purchase Reversal Journal Entry generated successfully for Invoice {InvoiceNo}.",
                    invoice.InvoiceNumber
                );

                return reversalEntry;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate reversal Purchase Journal Entry for Invoice {InvoiceNo}", invoice.InvoiceNumber);
                throw;
            }
        }

        public async Task<JournalEntry> CreatePurchasePaymentEntryAsync(
        PurchaseInvoice invoice,
        string CreatedBy,
        decimal amount,
        PaymentMethod method,
        CancellationToken cancellationToken)
        {
            var systemSettings = await _unitOfWork
                .GetRepository<SystemSetting, int>()
                .GetAllAsync(new GetAllSystemSettingsSpec(), cancellationToken);

            var accountsPayableId = Guid.Parse(_getSystemValues.GetSettingValue(systemSettings, "AccountsPayableAccountId"));
            var cashAccountId = Guid.Parse(_getSystemValues.GetSettingValue(systemSettings, "CashAccountId"));
            var bankAccountId = Guid.Parse(_getSystemValues.GetSettingValue(systemSettings, "BankAccountId"));

            var creditAccountId = method == PaymentMethod.Cash ? cashAccountId : bankAccountId;

            var journalNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken);

            var entry = new JournalEntry(
                journalEntryNumber: journalNumber,
                description: $"Payment for Purchase Invoice {invoice.InvoiceNumber}",
                entryDate: DateTime.UtcNow,
                transactionType: TransactionType.PurchasePayment,
                relatedEntityType: RelatedEntityType.PurchaseInvoicePayment,
                    relatedEntityId: invoice.Id,
                    createdBy: CreatedBy
            );

            var debitLine = new JournalEntryLine(accountsPayableId, amount, 0, "Accounts Payable");
            debitLine.AttachToJournalEntry(entry.Id);
            entry.Lines.Add(debitLine);

            var creditLine = new JournalEntryLine(creditAccountId, 0, amount, method.ToString());
            creditLine.AttachToJournalEntry(entry.Id);
            entry.Lines.Add(creditLine);

            entry.ValidateBalance();

            await _unitOfWork.GetRepository<JournalEntry, Guid>().AddAsync(entry, cancellationToken);

            return entry;
        }

        public async Task<JournalEntry> CreatePurchasePaymentReversalJournalAsync(
            PurchasePayment payment,
            PurchaseInvoice invoice,
            string reversedBy,
            CancellationToken cancellationToken)
        {
            if (payment is null)
                throw new ArgumentNullException(nameof(payment), "Payment cannot be null.");

            if (invoice is null)
                throw new ArgumentNullException(nameof(invoice), "Invoice cannot be null.");

            if (payment.JournalEntryId == Guid.Empty)
                throw new BadRequestException("Cannot create reversal: original JournalEntryId is missing.");

            var journalEntryRepo = _unitOfWork.GetRepository<JournalEntry, Guid>();
            var originalEntry = await journalEntryRepo.GetByIdAsync(payment.JournalEntryId, cancellationToken);

            if (originalEntry is null)
                throw new NotFoundException<JournalEntry>(payment.JournalEntryId);

            var reversalEntry = originalEntry.CreateReversal(reversedBy, $"Reversed JE for JE-Number : {originalEntry.JournalEntryNumber}");

            reversalEntry.JournalEntryNumber = await _numberSequenceService
                .GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken);

            reversalEntry.RelatedEntityId = invoice.Id;
            reversalEntry.RelatedEntityType = RelatedEntityType.PurchaseInvoice;

            reversalEntry.ValidateBalance();

            return reversalEntry;
        }

        public async Task CreateSalesInvoiceJournalAsync(
            SalesInvoice invoice,
            string CreatedBy,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting journal entry creation for SalesInvoice {InvoiceId} (InvoiceNumber: {InvoiceNumber})",
                invoice.Id, invoice.InvoiceNumber);

            var systemSettingsRepo = _unitOfWork.GetRepository<SystemSetting, int>();
            var systemSettings = await systemSettingsRepo.GetAllAsync(new GetAllSystemSettingsSpec(), cancellationToken);

            Guid arAccountId = GetAccountGuid(systemSettings, "AccountsReceivableAccountId");
            Guid revenueAccountId = GetAccountGuid(systemSettings, "SalesRevenueAccountId");
            Guid discountAccountId = GetAccountGuid(systemSettings, "DiscountAllowedAccountId");
            Guid taxAccountId = GetAccountGuid(systemSettings, "TaxPayableAccountId");

            _logger.LogDebug(
                "System accounts resolved for Journal Entry. AR: {AR}, Revenue: {REV}, Discount: {DISC}, Tax: {TAX}",
                arAccountId, revenueAccountId, discountAccountId, taxAccountId);

            var journalNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken);

            var journalEntry = new JournalEntry(
                journalEntryNumber: journalNumber,
                description: $"Sales Invoice for Invoice {invoice.InvoiceNumber}",
                entryDate: DateTime.UtcNow,
                transactionType: TransactionType.SalesInvoice,
                  relatedEntityType: RelatedEntityType.SalesInvoice,
                    relatedEntityId: invoice.Id,
                    createdBy: CreatedBy
            )
            {
                RelatedEntityType = RelatedEntityType.SalesInvoice,
                RelatedEntityId = invoice.Id
            };

            var arLine = new JournalEntryLine(arAccountId, invoice.GrandTotal.Round2(), 0, "Accounts Receivable");
            arLine.AttachToJournalEntry(journalEntry.Id);
            journalEntry.Lines.Add(arLine);

            var revenueLine = new JournalEntryLine(revenueAccountId, 0, invoice.SubTotal.Round2(), "Sales Revenue");
            revenueLine.AttachToJournalEntry(journalEntry.Id);
            journalEntry.Lines.Add(revenueLine);

            if (invoice.DiscountAmount > 0 && discountAccountId != Guid.Empty)
            {
                var discountLine = new JournalEntryLine(discountAccountId, invoice.DiscountAmount.Round2(), 0, "Discount Allowed");
                discountLine.AttachToJournalEntry(journalEntry.Id);
                journalEntry.Lines.Add(discountLine);
            }

            if (invoice.TaxAmount > 0 && taxAccountId != Guid.Empty)
            {
                var taxLine = new JournalEntryLine(taxAccountId, 0, invoice.TaxAmount.Round2(), "Tax Payable");
                taxLine.AttachToJournalEntry(journalEntry.Id);
                journalEntry.Lines.Add(taxLine);
            }

            journalEntry.ValidateBalance();

            _logger.LogInformation(
                "Journal entry {JournalNumber} created successfully for Invoice {InvoiceId}. Debit = Credit = {Total}",
                journalEntry.JournalEntryNumber, invoice.Id, journalEntry.Lines.Sum(l => l.Debit));

            invoice.JournalEntry = journalEntry;
        }



        private Guid GetAccountGuid(IEnumerable<SystemSetting> settings, string key)
        {
            var value = _getSystemValues.GetSettingValue(settings, key);
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"Missing required system setting: {key}");

            if (!Guid.TryParse(value, out var guid))
                throw new DomainException($"Invalid GUID in system setting: {key}");

            return guid;
        }

        public async Task<JournalEntry> CreateSalesInvoicePaymentJournalAsync(
    SalesInvoice invoice,
    string CreatedBy,
    CreateSalesInvoicePaymentCommand request,
    IEnumerable<SystemSetting> systemSettings,
    CancellationToken cancellationToken)
        {
            if (request.Dto.Amount <= 0)
            {
                _logger.LogWarning("Attempted to create journal entry with invalid amount: {Amount}", request.Dto.Amount);
                throw new BadRequestException("Payment amount must be greater than zero.");
            }

            var accountId = request.Dto.Method == PaymentMethod.Cash
                ? _getSystemValues.GetSystemSettingGuid(systemSettings, "CashAccountId")
                : _getSystemValues.GetSystemSettingGuid(systemSettings, "BankAccountId");

            var receivableAccountId = _getSystemValues.GetSystemSettingGuid(systemSettings, "AccountsReceivableAccountId");

            _logger.LogInformation(
                "Creating sales payment journal entry for Invoice {InvoiceNumber}, Amount {Amount}, Method {Method}",
                invoice.InvoiceNumber,
                request.Dto.Amount,
                request.Dto.Method);

            var journalNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken);

            var journalEntry = new JournalEntry(
               journalEntryNumber: journalNumber,
               description: $"Sales Payment for Invoice {invoice.InvoiceNumber}",
               entryDate: DateTime.UtcNow,
               transactionType: TransactionType.SalesPayment,
                                   relatedEntityType: RelatedEntityType.SalesInvoicePayment,
                    relatedEntityId: invoice.Id,
                    createdBy: CreatedBy
           )
            {
                CreatedBy = request.CreatedBy ?? "System"
            };

            var debitLine = new JournalEntryLine(accountId, request.Dto.Amount, 0m, "Cash/Bank Payment");
            debitLine.AttachToJournalEntry(journalEntry.Id);
            journalEntry.Lines.Add(debitLine);

            var creditLine = new JournalEntryLine(receivableAccountId, 0m, request.Dto.Amount, "Accounts Receivable");
            creditLine.AttachToJournalEntry(journalEntry.Id);
            journalEntry.Lines.Add(creditLine);

            journalEntry.ValidateBalance();

            _logger.LogInformation(
                "Journal entry created successfully with number {JournalEntryNumber} for invoice {InvoiceNumber}",
                journalEntry.JournalEntryNumber,
                invoice.InvoiceNumber);

            return journalEntry;
        }

        public async Task<JournalEntry> CreateReverseSalesPaymentJournalEntry(SalesInvoicePayment payment, string CreatedBy, CancellationToken cancellationToken)
        {
            if (payment.JournalEntry == null)
                throw new BadRequestException($"No journal entry found for payment {payment.Id}");

            var journalNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken);

            var reversalEntry = new JournalEntry(
                journalEntryNumber: journalNumber,
                description: $"Reversal of Sales Payment {payment.Id}",
                entryDate: DateTime.UtcNow,
                transactionType: TransactionType.SalesPaymentReversal,
                                    relatedEntityType: RelatedEntityType.PurchaseInvoice,
                    relatedEntityId: payment.Id,
                    createdBy: CreatedBy
            );

            foreach (var line in payment.JournalEntry.Lines)
            {
                var reversalLine = new JournalEntryLine(
                    accountId: line.AccountId,
                    debit: line.Credit,
                    credit: line.Debit,
                    description: $"Reversal of line {line.Id}"
                );

                reversalLine.AttachToJournalEntry(reversalEntry.Id);
                reversalEntry.Lines.Add(reversalLine);
            }

            reversalEntry.ValidateBalance();

            return reversalEntry;
        }

        public void UpdateBalances(IEnumerable<Account> accounts, JournalEntry entry)
        {
            var accountsDict = accounts.ToDictionary(a => a.Id, a => a);
            foreach (var line in entry.Lines)
            {
                var account = accountsDict[line.AccountId];

                decimal amountChange = 0m;

                switch (account.AccountType)
                {
                    case AccountType.Asset:
                    case AccountType.Expense:
                        amountChange = line.Debit - line.Credit;
                        account.Balance += amountChange;
                        break;

                    case AccountType.Liability:
                    case AccountType.Revenue:
                    case AccountType.Equity:
                        amountChange = line.Credit - line.Debit;
                        account.Balance += amountChange;
                        break;
                }

                account.ModifiedAt = DateTime.UtcNow;

                if (account.SafeId.HasValue && account.Safe != null)
                {
                    account.Safe.Balance += amountChange;

                    var safeTransaction = new SafeTransaction
                    {
                        SafeId = account.SafeId.Value,
                        Date = DateTime.UtcNow,
                        Amount = Math.Abs(amountChange),
                        Type = GetSafeTransactionType(entry.TransactionType, amountChange),
                        JournalEntryId = entry.Id,
                        Note = $"Journal Entry {entry.JournalEntryNumber} affecting account {account.Code}",
                        SourceId = entry.Id,
                        SourceType = GetTransactionSourceType(entry.TransactionType)
                    };

                    account.Safe.SafeTransactions.Add(safeTransaction);
                }
            }
        }
        public void ReverseUpdateBalances(IEnumerable<Account> accounts, JournalEntry reversedEntry)
        {
            var accountsDict = accounts.ToDictionary(a => a.Id, a => a);

            foreach (var line in reversedEntry.Lines)
            {
                var account = accountsDict[line.AccountId];

                decimal amountChange = 0m;

                switch (account.AccountType)
                {
                    case AccountType.Asset:
                    case AccountType.Expense:
                        amountChange = line.Debit - line.Credit;
                        break;

                    case AccountType.Liability:
                    case AccountType.Revenue:
                    case AccountType.Equity:
                        amountChange = line.Credit - line.Debit;
                        break;
                }

                account.Balance += amountChange;

                if (account.SafeId.HasValue && account.Safe != null)
                {
                    account.Safe.Balance += amountChange;
                }
            }
        }
        private SafeTransactionType GetSafeTransactionType(TransactionType transactionType, decimal amountChange)
        {
            return transactionType switch
            {
                TransactionType.SalesPayment => amountChange > 0 ? SafeTransactionType.SalesPayment : SafeTransactionType.SalesPaymentReversal,
                TransactionType.PurchasePayment => amountChange > 0 ? SafeTransactionType.PurchasePayment : SafeTransactionType.PurchasePaymentReversal,
                _ => SafeTransactionType.Transfer
            };
        }

        private TransactionSourceType GetTransactionSourceType(TransactionType transactionType)
        {
            return transactionType switch
            {
                TransactionType.SalesPayment => TransactionSourceType.SalesInvoicePayment,
                TransactionType.PurchasePayment => TransactionSourceType.PurchaseInvoicePayment,
                TransactionType.SalaryExpense => TransactionSourceType.Salary,
                _ => TransactionSourceType.Transfer
            };
        }

    }
    public static class DecimalExtensions
    {
        public static decimal Round2(this decimal value) => Math.Round(value, 2);
    }
}

