using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.Interfaces.ISalesModule;
using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Create;
using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Delete;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services
{
    internal class SafeTransactionService(IUnitOfWork unitOfWork, IGetSystemValues getSystemValues, ILogger<SafeTransactionService> logger)
        : ISafeTransactionService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGetSystemValues _getSystemValues = getSystemValues;
        private readonly ILogger<SafeTransactionService> _logger = logger;

        public async Task ApplySalesPaymentTransactionAsync(
            SalesInvoicePayment payment,
            JournalEntry journalEntry,
            CreateSalesInvoicePaymentCommand request,
            string invoiceNumber,
            IEnumerable<SystemSetting> systemSettings,
            CancellationToken cancellationToken)
        {
            if (request.Dto.Amount <= 0)
            {
                _logger.LogWarning("Invalid sales payment amount: {Amount}", request.Dto.Amount);
                throw new BadRequestException("Payment amount must be greater than zero.");
            }

            var safeId = request.Dto.Method == PaymentMethod.Cash
                ? _getSystemValues.GetSystemSettingGuid(systemSettings, "CashSafeId")
                : _getSystemValues.GetSystemSettingGuid(systemSettings, "BankSafeId");

            if (safeId == Guid.Empty)
            {
                _logger.LogError("SafeId configuration is missing for payment method {Method}", request.Dto.Method);
                throw new BadRequestException("Safe configuration is missing.");
            }

            _logger.LogInformation(
                "Applying sales payment transaction for Invoice {InvoiceNumber}, Amount {Amount}, SafeId {SafeId}",
                invoiceNumber, request.Dto.Amount, safeId);

            var safeRepo = _unitOfWork.GetRepository<Safe, Guid>();
            var safe = await safeRepo.GetByIdAsync(safeId, cancellationToken);
            if (safe == null)
            {
                _logger.LogError("Safe not found with Id: {SafeId}", safeId);
                throw new NotFoundException<Safe>(safeId);
            }

            var safeTransaction = new SafeTransaction
            {
                Id = Guid.NewGuid(),
                Amount = request.Dto.Amount,
                Note = $"Sales Payment for Invoice {invoiceNumber}",
                SafeId = safeId,
                Type = SafeTransactionType.SalesPayment,
                CreatedAt = DateTime.UtcNow,
                SourceId = payment.Id,
                JournalEntryId = journalEntry.Id,
                CreatedBy = request.CreatedBy,
                SourceType = TransactionSourceType.SalesInvoicePayment
            };

            safe.Balance += request.Dto.Amount;
            safe.SafeTransactions.Add(safeTransaction);

            payment.JournalEntry = journalEntry;
            payment.SafeTransactions.Add(safeTransaction);

            _logger.LogInformation(
                "Sales payment applied successfully. SafeId: {SafeId}, New Balance: {Balance}, PaymentId: {PaymentId}",
                safeId, safe.Balance, payment.Id);
        }
        public async Task SalesPaymentSafeReversal(DeleteSalesInvoicePaymentCommand request, SalesInvoicePayment payment, JournalEntry reversalJournal, IGenericRepository<Safe, Guid> safeRepo, CancellationToken cancellationToken)
        {
            foreach (var oldSafeTx in payment.SafeTransactions.ToList())
            {
                var safe = await safeRepo.GetByIdAsync(oldSafeTx.SafeId, cancellationToken);
                if (safe == null)
                {
                    _logger.LogError("Safe {SafeId} not found for reversal of payment {PaymentId}", oldSafeTx.SafeId, payment.Id);
                    throw new NotFoundException<Safe>(oldSafeTx.SafeId);
                }

                var reversalSafeTx = new SafeTransaction
                {
                    Id = Guid.NewGuid(),
                    Amount = -oldSafeTx.Amount,
                    Note = $"Reversal of Sales Payment {payment.Id}",
                    SafeId = safe.Id,
                    CreatedAt = DateTime.UtcNow,
                    Type = SafeTransactionType.SalesPaymentReversal,
                    JournalEntryId = reversalJournal.Id,
                    CreatedBy = request.DeletedBy,
                    SourceId = payment.Id,
                    SourceType = TransactionSourceType.SalesInvoicePayment
                };

                safe.Balance -= oldSafeTx.Amount;
                safe.SafeTransactions.Add(reversalSafeTx);
                payment.SafeTransactions.Add(reversalSafeTx);
            }
            _logger.LogInformation("Sales payment reversal processed for PaymentId: {PaymentId}", payment.Id);
        }
    }
}
