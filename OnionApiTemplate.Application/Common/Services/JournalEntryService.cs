using Khazen.Application.Common.Interfaces;
using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Create;
using Khazen.Domain.Common.Enums;
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

            var journalEntry = new JournalEntry
            {
                CreatedAt = DateTime.UtcNow,
                EntryDate = DateTime.UtcNow,
                TransactionType = TransactionType.SalaryExpense,
                Description = $"Salary for {employee.FirstName} {employee.LastName} - {salary.SalaryDate:MMMM yyyy}, created by '{createdBy}'",
                JournalEntryNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken),
                Lines = new List<JournalEntryLine>
                {
                    new()
                    {
                        AccountId = salaryExpenseAccountId,
                        Debit = salary.NetSalary,
                        Credit = 0m,
                        Description = "Salary Expense"
                    },
                    new()
                    {
                        AccountId = cashAccountId,
                        Debit = 0m,
                        Credit = salary.NetSalary,
                        Description = "Cash Payment"
                    }
                }
            };

            _logger.LogInformation("Journal entry created for salary ID {SalaryId}", salary.Id);
            return journalEntry;
        }

        public async Task<JournalEntry> GeneratePurchaseJournalEntryAsync(
        PurchaseInvoice invoice,
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

                var journal = new JournalEntry
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    EntryDate = DateTime.UtcNow,
                    Description = $"Purchase Invoice {invoice.InvoiceNumber}",
                    JournalEntryNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken),
                    TransactionType = TransactionType.PurchaseInvoice,
                    RelatedEntityType = RelatedEntityType.PurchaseInvoice,
                    RelatedEntityId = invoice.Id,
                    Lines = new List<JournalEntryLine>()
                };

                journal.Lines.Add(new JournalEntryLine
                {
                    AccountId = purchasesAccountId,
                    Debit = invoice.TotalAmount,
                    Credit = 0,
                    Description = "Purchases"
                });

                journal.Lines.Add(new JournalEntryLine
                {
                    AccountId = accountsPayableId,
                    Debit = 0,
                    Credit = invoice.TotalAmount,
                    Description = "Accounts Payable"
                });

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

        public async Task<JournalEntry> GenerateReversalPurchaseInvoiceAsync(PurchaseInvoice invoice, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Generating Reversal Purchase Journal Entry for Invoice {InvoiceNo}", invoice.InvoiceNumber);
                cancellationToken.ThrowIfCancellationRequested();

                var systemSettingsRepo = _unitOfWork.GetRepository<SystemSetting, int>();
                var systemSettings = await systemSettingsRepo.GetAllAsync(new GetAllSystemSettingsSpec(), cancellationToken);

                var purchasesAccountId = Guid.Parse(_getSystemValues.GetSettingValue(systemSettings, "PurchasesAccountId"));
                var accountsPayableId = Guid.Parse(_getSystemValues.GetSettingValue(systemSettings, "AccountsPayableAccountId"));

                var reversalEntry = new JournalEntry
                {
                    EntryDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Description = $"Reversal of Purchase Invoice {invoice.InvoiceNumber}",
                    JournalEntryNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken),
                    TransactionType = TransactionType.PurchaseInvoiceReversal,
                    RelatedEntityType = RelatedEntityType.PurchaseInvoice,
                    RelatedEntityId = invoice.Id,
                    Lines = new List<JournalEntryLine>
                        {
                            new JournalEntryLine
                            {
                                AccountId = accountsPayableId,
                                Debit = invoice.TotalAmount,
                                Credit = 0
                            },
                            new JournalEntryLine
                            {
                                AccountId = purchasesAccountId,
                                Debit = 0,
                                Credit = invoice.TotalAmount
                            }
                        }
                };

                _logger.LogInformation(
                    "Purchase Journal Entry generated successfully for Invoice {InvoiceNo}.",
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
        public async Task<JournalEntry> CreatePurchasePaymentEntryAsync(PurchaseInvoice invoice, decimal amount, PaymentMethod method, CancellationToken cancellationToken)
        {
            var systemSettings = await _unitOfWork
                .GetRepository<SystemSetting, int>()
                .GetAllAsync(new GetAllSystemSettingsSpec(), cancellationToken);

            var accountsPayableId = Guid.Parse(_getSystemValues.GetSettingValue(systemSettings, "AccountsPayableAccountId"));
            var cashAccountId = Guid.Parse(_getSystemValues.GetSettingValue(systemSettings, "CashAccountId"));
            var bankAccountId = Guid.Parse(_getSystemValues.GetSettingValue(systemSettings, "BankAccountId"));

            var creditAccountId = method == PaymentMethod.Cash ? cashAccountId : bankAccountId;

            var entry = new JournalEntry
            {
                CreatedAt = DateTime.UtcNow,
                EntryDate = DateTime.UtcNow,
                Description = $"Payment for Purchase Invoice {invoice.InvoiceNumber}",
                JournalEntryNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken),
                TransactionType = TransactionType.PurchasePayment,
                RelatedEntityId = invoice.Id,
                RelatedEntityType = RelatedEntityType.PurchaseInvoice,
                Lines =
                [
                    new JournalEntryLine
            {
                AccountId = accountsPayableId,
                Debit = amount,
                Credit = 0
            },

            new JournalEntryLine
            {
                AccountId = creditAccountId,
                Debit = 0,
                Credit = amount
            }
                ]
            };

            await _unitOfWork.GetRepository<JournalEntry, Guid>().AddAsync(entry, cancellationToken);
            return entry;
        }
        public async Task<JournalEntry> CreatePrchasePaymentReversalJournalAsync(PurchasePayment payment, PurchaseInvoice invoice, CancellationToken cancellationToken)
        {
            if (payment is null)
                throw new ArgumentNullException(nameof(payment), "Payment cannot be null.");

            if (invoice is null)
                throw new ArgumentNullException(nameof(invoice), "Invoice cannot be null.");

            if (payment.JournalEntryId == Guid.Empty)
                throw new BadRequestException("Cannot create reversal: original JournalEntryId is missing.");

            var systemSettings = await _unitOfWork
                .GetRepository<SystemSetting, int>()
                .GetAllAsync(new GetAllSystemSettingsSpec(), cancellationToken);

            if (systemSettings is null || !systemSettings.Any())
                throw new BadRequestException("System settings are missing. Cannot create reversal journal entry.");



            var accountCashId = GetSafeSystemGuid("CashAccountId", systemSettings);
            var accountBankId = GetSafeSystemGuid("BankAccountId", systemSettings);
            var accountPayableId = GetSafeSystemGuid("AccountsPayableAccountId", systemSettings);


            var reversalAccountId = payment.Method == PaymentMethod.Cash
                ? accountCashId
                : accountBankId;


            var lines = new List<JournalEntryLine>
                    {

                        new JournalEntryLine
                        {
                            AccountId = reversalAccountId,
                            Debit = payment.Amount,
                            Credit = 0
                        },

                            new JournalEntryLine
                        {
                            AccountId = accountPayableId,
                            Debit = 0,
                            Credit = payment.Amount
                        }
                    };


            var journalEntry = new JournalEntry
            {
                EntryDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Description = $"Reversal of Payment {payment.Id} for Invoice {invoice.InvoiceNumber}",
                JournalEntryNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken),
                TransactionType = TransactionType.PurchaseInvoiceReversal,
                ReversalOfJournalEntryId = payment.JournalEntryId,
                Lines = lines
            };

            return journalEntry;
        }
        Guid GetSafeSystemGuid(string key, IEnumerable<SystemSetting> systemSettings)
        {
            var value = _getSystemValues.GetSettingValue(systemSettings, key);

            if (string.IsNullOrWhiteSpace(value))
                throw new BadRequestException($"System setting '{key}' is missing.");

            if (!Guid.TryParse(value, out Guid result))
                throw new BadRequestException($"System setting '{key}' contains invalid GUID value.");

            return result;
        }

        public async Task CreateSalesInvoiceJournalAsync(SalesInvoice salesInvoice, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting journal entry creation for SalesInvoice {InvoiceId} (InvoiceNumber: {InvoiceNumber})",
                salesInvoice.Id, salesInvoice.InvoiceNumber);

            var systemSettingsRepo = _unitOfWork.GetRepository<SystemSetting, int>();
            var systemSettings = await systemSettingsRepo.GetAllAsync(new GetAllSystemSettingsSpec(), cancellationToken);

            Guid arAccountId = GetAccountGuid(systemSettings, "AccountsReceivableAccountId");
            Guid revenueAccountId = GetAccountGuid(systemSettings, "SalesRevenueAccountId");
            Guid discountAccountId = _getSystemValues.GetSystemSettingGuid(systemSettings, "DiscountAllowedAccountId");
            Guid taxAccountId = _getSystemValues.GetSystemSettingGuid(systemSettings, "TaxPayableAccountId");

            _logger.LogDebug("System accounts resolved for Journal Entry. AR: {AR}, Revenue: {REV}, Discount: {DISC}, Tax: {TAX}",
                arAccountId, revenueAccountId, discountAccountId, taxAccountId);

            var journalEntry = new JournalEntry
            {
                EntryDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Description = $"Sales Invoice for Invoice {salesInvoice.InvoiceNumber}",
                JournalEntryNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken),
                TransactionType = TransactionType.SalesInvoice,
                RelatedEntityType = RelatedEntityType.SalesInvoice,
                RelatedEntityId = salesInvoice.Id,
                Lines = new List<JournalEntryLine>()
            };

            journalEntry.Lines.Add(new JournalEntryLine
            {
                AccountId = arAccountId,
                Debit = salesInvoice.GrandTotal.Round2(),
                Credit = 0
            });

            journalEntry.Lines.Add(new JournalEntryLine
            {
                AccountId = revenueAccountId,
                Debit = 0,
                Credit = salesInvoice.SubTotal.Round2()
            });

            if (salesInvoice.DiscountAmount > 0 && discountAccountId != Guid.Empty)
            {
                journalEntry.Lines.Add(new JournalEntryLine
                {
                    AccountId = discountAccountId,
                    Debit = salesInvoice.DiscountAmount.Round2(),
                    Credit = 0
                });
            }

            if (salesInvoice.TaxAmount > 0 && taxAccountId != Guid.Empty)
            {
                journalEntry.Lines.Add(new JournalEntryLine
                {
                    AccountId = taxAccountId,
                    Debit = 0,
                    Credit = salesInvoice.TaxAmount.Round2()
                });
            }

            var totalDebit = journalEntry.Lines.Sum(x => x.Debit);
            var totalCredit = journalEntry.Lines.Sum(x => x.Credit);

            if (totalDebit != totalCredit)
            {
                _logger.LogError(
                    "Journal entry imbalance detected for Invoice {InvoiceId}. Debit: {Debit}, Credit: {Credit}",
                    salesInvoice.Id, totalDebit, totalCredit);

                throw new BadRequestException("Journal entry is not balanced.");
            }

            _logger.LogInformation(
                "Journal entry {JournalNumber} created successfully for Invoice {InvoiceId}. Debit = Credit = {Total}",
                journalEntry.JournalEntryNumber, salesInvoice.Id, totalDebit);

            salesInvoice.JournalEntry = journalEntry;
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
             SalesInvoice salesInvoice,
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
                salesInvoice.InvoiceNumber,
                request.Dto.Amount,
                request.Dto.Method);

            var journalEntry = new JournalEntry
            {
                Id = Guid.NewGuid(),
                EntryDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Description = $"Sales Payment for Invoice {salesInvoice.InvoiceNumber}",
                JournalEntryNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken),
                TransactionType = TransactionType.SalesPayment,
                CreatedBy = request.CreatedBy ?? "System",
                Lines = new List<JournalEntryLine>
        {
            new JournalEntryLine
            {
                AccountId = accountId,
                Debit = request.Dto.Amount,
                Credit = 0
            },
            new JournalEntryLine
            {
                AccountId = receivableAccountId,
                Debit = 0,
                Credit = request.Dto.Amount
            }
        }
            };

            _logger.LogInformation(
                "Journal entry created successfully with number {JournalEntryNumber} for invoice {Invoice}",
                journalEntry.JournalEntryNumber,
                salesInvoice.InvoiceNumber);

            return journalEntry;
        }
        public async Task<JournalEntry> CreateReverseSalesPaymenttJournalEntry(SalesInvoicePayment payment, CancellationToken cancellationToken)
        {
            return new JournalEntry
            {
                Id = Guid.NewGuid(),
                EntryDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Description = $"Reversal of Sales Payment {payment.Id}",
                TransactionType = TransactionType.SalesPaymentReversal,
                JournalEntryNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken),
                Lines = payment.JournalEntry!.Lines
                                    .Select(line => new JournalEntryLine
                                    {
                                        AccountId = line.AccountId,
                                        Debit = line.Credit,
                                        Credit = line.Debit
                                    }).ToList()
            };
        }
    }
    public static class DecimalExtensions
    {
        public static decimal Round2(this decimal value) => Math.Round(value, 2);
    }
}

