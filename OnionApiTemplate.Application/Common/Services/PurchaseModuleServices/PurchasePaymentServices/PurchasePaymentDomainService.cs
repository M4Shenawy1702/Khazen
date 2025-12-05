using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchasePaymentServices;
using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services.PurchaseModuleServices.PurchasePaymentServices
{
    public class PurchasePaymentDomainService(
        IUnitOfWork unitOfWork,
        IGetSystemValues systemValues,
        IJournalEntryService journalEntryService,
        ILogger<PurchasePaymentDomainService> logger
           ) : IPurchasePaymentDomainService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IGetSystemValues _systemValues = systemValues;
        private readonly IJournalEntryService _journalEntryService = journalEntryService;
        private readonly ILogger<PurchasePaymentDomainService> _logger = logger;

        public async Task<PurchasePayment> CreatePaymentAsync(PurchaseInvoice invoice, CreatePurchasePaymentDto Dto, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start CreatePaymentAsync | InvoiceId={InvoiceId}, Amount={Amount}, Method={Method}, CurrentPaid={Paid}, Total={Total}",
                invoice.Id, Dto.Amount, Dto.Method, invoice.PaidAmount, invoice.TotalAmount);

            if (invoice.PaymentStatus == PaymentStatus.Paid)
            {
                _logger.LogWarning("Invoice already paid | InvoiceId={InvoiceId}", invoice.Id);
                throw new BadRequestException("Invoice already fully paid.");
            }

            var alreadyPaid = invoice.Payments.Where(p => !p.IsReversed).Sum(p => p.Amount);
            var remaining = invoice.TotalAmount - alreadyPaid;

            _logger.LogDebug("Remaining amount calculated | InvoiceId={InvoiceId}, Remaining={Remaining}", invoice.Id, remaining);

            if (Dto.Amount > remaining)
            {
                _logger.LogWarning(
                    "Payment exceeds remaining amount | InvoiceId={InvoiceId}, Amount={Amount}, Remaining={Remaining}",
                    invoice.Id, Dto.Amount, remaining);

                throw new BadRequestException($"Payment exceeds remaining amount ({remaining}).");
            }

            _logger.LogDebug("Fetching system settings for payment processing...");
            var systemSettings = await _unitOfWork
                .GetRepository<SystemSetting, int>()
                .GetAllAsync(new GetAllSystemSettingsSpec(), cancellationToken);

            _logger.LogDebug("Creating Journal Entry for payment | InvoiceId={InvoiceId}", invoice.Id);
            var journalEntry = await _journalEntryService.CreatePurchasePaymentEntryAsync(
                invoice, Dto.Amount, Dto.Method, cancellationToken);

            var safeKey = Dto.Method == PaymentMethod.Cash ? "CashSafeId" : "BankSafeId";
            var safeId = Guid.Parse(_systemValues.GetSettingValue(systemSettings, safeKey));

            _logger.LogDebug("Selected safe | SafeKey={SafeKey}, SafeId={SafeId}", safeKey, safeId);

            var safeRepo = _unitOfWork.GetRepository<Safe, Guid>();
            var safe = await safeRepo.GetByIdAsync(safeId, cancellationToken);

            if (safe is null)
            {
                _logger.LogWarning("Safe not found | SafeId={SafeId}", safeId);
                throw new NotFoundException<Safe>(safeId);
            }

            _logger.LogDebug("Safe found | SafeId={SafeId}, Balance={Balance}", safeId, safe.Balance);

            if (safe.Balance < Dto.Amount)
            {
                _logger.LogWarning("Insufficient safe balance | SafeId={SafeId}, Balance={Balance}, Requested={Amount}",
                    safeId, safe.Balance, Dto.Amount);

                throw new BadRequestException("Safe balance is insufficient.");
            }

            var safeTransaction = new SafeTransaction
            {
                Amount = Dto.Amount,
                Note = $"Purchase Payment for Invoice {invoice.InvoiceNumber}",
                SafeId = safeId,
                Type = SafeTransactionType.PurchasePayment,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogDebug("Safe transaction created | SafeId={SafeId}, Amount={Amount}", safeId, Dto.Amount);

            var payment = new PurchasePayment(
                amount: Dto.Amount,
                method: Dto.Method,
                invoice: invoice,
                safeTransaction: safeTransaction
            );

            payment.AssignJournalEntry(journalEntry.Id);

            _logger.LogDebug("PurchasePayment entity created | PaymentId={PaymentId}", payment.Id);

            safe.Balance -= Dto.Amount;
            safeRepo.Update(safe);

            _logger.LogInformation("Safe balance updated | SafeId={SafeId}, NewBalance={Balance}", safeId, safe.Balance);

            invoice.RecalculateTotals();

            _logger.LogInformation(
                "Invoice totals recalculated | InvoiceId={InvoiceId}, Paid={Paid}, Remaining={Remaining}, Status={Status}",
                invoice.Id, invoice.PaidAmount, invoice.RemainingAmount, invoice.PaymentStatus);

            await _unitOfWork.GetRepository<JournalEntry, Guid>().AddAsync(journalEntry, cancellationToken);
            await _unitOfWork.GetRepository<SafeTransaction, Guid>().AddAsync(safeTransaction, cancellationToken);
            await _unitOfWork.GetRepository<PurchasePayment, Guid>().AddAsync(payment, cancellationToken);

            _logger.LogInformation(
                "Payment creation completed successfully | InvoiceId={InvoiceId}, PaymentId={PaymentId}, Amount={Amount}",
                invoice.Id, payment.Id, Dto.Amount);

            return payment;
        }
    }
}
