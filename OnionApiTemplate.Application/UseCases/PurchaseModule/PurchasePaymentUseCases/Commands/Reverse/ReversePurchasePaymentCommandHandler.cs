using Khazen.Application.Common.Interfaces;
using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications;
using Khazen.Application.Specification.PurchaseModule.PurchasePaymentSpecificatiocs;
using Khazen.Domain.Entities;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Commands.Delete
{
    public class ReversePurchasePaymentCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        ILogger<ReversePurchasePaymentCommandHandler> logger,
        IJournalEntryService journalEntryService)
        : IRequestHandler<ReversePurchasePaymentCommand, PurchasePaymentDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<ReversePurchasePaymentCommandHandler> _logger = logger;
        private readonly IJournalEntryService _journalEntryService = journalEntryService;

        public async Task<PurchasePaymentDto> Handle(
            ReversePurchasePaymentCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting reversal of PurchasePayment {PaymentId} by user {User}",
                request.Id, request.CurrentUserId);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction started");

            try
            {
                _logger.LogDebug("Validating user {User}", request.CurrentUserId);

                var user = await _userManager.FindByNameAsync(request.CurrentUserId);
                if (user is null)
                {
                    _logger.LogWarning("User {User} not found", request.CurrentUserId);
                    throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
                }

                _logger.LogInformation("User {UserId} validated", user.Id);

                var paymentRepo = _unitOfWork.GetRepository<PurchasePayment, Guid>();
                var invoiceRepo = _unitOfWork.GetRepository<PurchaseInvoice, Guid>();
                var journalRepo = _unitOfWork.GetRepository<JournalEntry, Guid>();
                var safeRepo = _unitOfWork.GetRepository<Safe, Guid>();

                _logger.LogDebug("Loading PurchasePayment {PaymentId}", request.Id);

                var payment = await paymentRepo.GetAsync(
                    new GetPurcasePaymentByIdWithIncludesSpecifications(request.Id),
                    cancellationToken);

                if (payment is null)
                {
                    _logger.LogWarning("PurchasePayment {PaymentId} not found", request.Id);
                    throw new NotFoundException<PurchasePayment>(request.Id);
                }

                if (payment.IsReversed)
                {
                    _logger.LogWarning("PurchasePayment {PaymentId} already reversed", payment.Id);
                    throw new BadRequestException("Payment is already reversed.");
                }

                _logger.LogInformation(
                    "PurchasePayment {PaymentId} loaded successfully",
                    payment.Id);

                _logger.LogDebug(
                    "Loading PurchaseInvoice {InvoiceId}",
                    payment.PurchaseInvoiceId);

                var invoice = await invoiceRepo.GetAsync(
                    new GetPurchaseInvoiceWithAllIncludesByIdSpec(payment.PurchaseInvoiceId),
                    cancellationToken);

                if (invoice is null)
                {
                    _logger.LogWarning(
                        "PurchaseInvoice {InvoiceId} not found",
                        payment.PurchaseInvoiceId);

                    throw new NotFoundException<PurchaseInvoice>(payment.PurchaseInvoiceId);
                }

                _logger.LogInformation(
                    "Invoice {InvoiceNumber} loaded",
                    invoice.InvoiceNumber);

                if (payment.SafeTransaction is null)
                {
                    _logger.LogWarning(
                        "Payment {PaymentId} has no SafeTransaction",
                        payment.Id);

                    throw new BadRequestException("Payment has no safe transaction.");
                }

                _logger.LogDebug(
                    "Loading Safe {SafeId}",
                    payment.SafeTransaction.SafeId);

                var safe = await safeRepo.GetByIdAsync(
                    payment.SafeTransaction.SafeId,
                    cancellationToken);

                if (safe is null)
                {
                    _logger.LogWarning(
                        "Safe {SafeId} not found",
                        payment.SafeTransaction.SafeId);

                    throw new NotFoundException<Safe>(payment.SafeTransaction.SafeId);
                }

                _logger.LogInformation(
                    "Safe {SafeId} loaded with balance {Balance}",
                    safe.Id, safe.Balance);

                _logger.LogDebug("Applying concurrency tokens");

                _unitOfWork.SetOriginalRowVersion(payment, request.RowVersion);

                _logger.LogInformation(
                    "Creating reversal journal entry for Payment {PaymentId}",
                    payment.Id);

                var reversalEntry =
                    await _journalEntryService.CreatePurchasePaymentReversalJournalAsync(
                        payment, invoice, user.Id, cancellationToken);

                await journalRepo.AddAsync(reversalEntry, cancellationToken);

                _logger.LogInformation(
                    "Reversal JournalEntry {JournalEntryId} created",
                    reversalEntry.Id);

                payment.Reverse(reversalEntry.Id, user.Id);
                invoice.RecalculateTotals();

                safe.Balance += payment.Amount;
                safe.ModifiedAt = DateTime.UtcNow;

                _logger.LogDebug(
                    "Safe {SafeId} balance updated to {Balance}",
                    safe.Id, safe.Balance);

                paymentRepo.Update(payment);
                invoiceRepo.Update(invoice);
                safeRepo.Update(safe);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation(
                    "Transaction committed successfully for Payment {PaymentId}",
                    payment.Id);

                return _mapper.Map<PurchasePaymentDto>(payment);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                _logger.LogWarning(
                    ex,
                    "Concurrency conflict while reversing Payment {PaymentId}",
                    request.Id);

                throw new ConflictException(
                    "The payment or related data was modified by another user. Please refresh and try again.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                _logger.LogError(
                    ex,
                    "Unexpected error while reversing Payment {PaymentId}",
                    request.Id);

                throw;
            }
        }

    }
}
