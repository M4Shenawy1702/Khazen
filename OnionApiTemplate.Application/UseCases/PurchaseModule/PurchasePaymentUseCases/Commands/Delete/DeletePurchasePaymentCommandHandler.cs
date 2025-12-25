using Khazen.Application.Common.Interfaces;
using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications;
using Khazen.Application.Specification.PurchaseModule.PurchasePaymentSpecificatiocs;
using Khazen.Domain.Entities;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Commands.Delete
{
    public class DeletePurchasePaymentCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        ILogger<DeletePurchasePaymentCommandHandler> logger,
        IJournalEntryService journalEntryService)
        : IRequestHandler<DeletePurchasePaymentCommand, PurchasePaymentDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<DeletePurchasePaymentCommandHandler> _logger = logger;
        private readonly IJournalEntryService _journalEntryService = journalEntryService;

        public async Task<PurchasePaymentDto> Handle(DeletePurchasePaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting deletion of PurchasePayment {PaymentId} by user {User}", request.Id, request.ReversedBy);
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var user = await _userManager.FindByNameAsync(request.ReversedBy);
                if (user is null)
                {
                    _logger.LogWarning("User {User} not found", request.ReversedBy);
                    throw new NotFoundException<ApplicationUser>(request.ReversedBy);
                }

                _logger.LogInformation("User validated: {User}", request.ReversedBy);

                var paymentRepo = _unitOfWork.GetRepository<PurchasePayment, Guid>();
                var invoiceRepo = _unitOfWork.GetRepository<PurchaseInvoice, Guid>();
                var journalRepo = _unitOfWork.GetRepository<JournalEntry, Guid>();
                var safeRepo = _unitOfWork.GetRepository<Safe, Guid>();

                var payment = await paymentRepo.GetAsync(
                    new GetPurcasePaymentByIdWithIncludesSpecifications(request.Id),
                    cancellationToken);

                if (payment is null)
                {
                    _logger.LogWarning("Payment {PaymentId} not found", request.Id);
                    throw new NotFoundException<PurchasePayment>(request.Id);
                }

                if (payment.IsReversed)
                {
                    _logger.LogWarning("Payment {PaymentId} is already reversed", payment.Id);
                    throw new BadRequestException("Payment is already deleted.");
                }

                var invoice = await invoiceRepo.GetAsync(
                    new GetPurchaseInvoiceWithAllIncludesByIdSpec(payment.PurchaseInvoiceId),
                    cancellationToken);

                if (invoice is null)
                {
                    _logger.LogWarning("Invoice {InvoiceId} not found", payment.PurchaseInvoiceId);
                    throw new NotFoundException<PurchaseInvoice>(payment.PurchaseInvoiceId);
                }

                _logger.LogInformation("Invoice {InvoiceNumber} loaded for payment reversal", invoice.InvoiceNumber);

                var reversalEntry = await _journalEntryService
                    .CreatePurchasePaymentReversalJournalAsync(payment, invoice, request.ReversedBy, cancellationToken);

                await journalRepo.AddAsync(reversalEntry, cancellationToken);
                _logger.LogInformation("Reversal JournalEntry {JournalEntryId} created for Payment {PaymentId}", reversalEntry.Id, payment.Id);

                if (payment.SafeTransaction is null)
                {
                    _logger.LogWarning("Payment {PaymentId} has no SafeTransaction", payment.Id);
                    throw new BadRequestException("Payment has no safe transaction.");
                }

                var safe = await safeRepo.GetByIdAsync(payment.SafeTransaction.SafeId, cancellationToken);

                if (safe is null)
                {
                    _logger.LogWarning("Safe {SafeId} not found", payment.SafeTransaction.SafeId);
                    throw new NotFoundException<Safe>(payment.SafeTransaction.SafeId);
                }

                safe.Balance += payment.Amount;
                safe.ModifiedAt = DateTime.UtcNow;
                safeRepo.Update(safe);

                _logger.LogInformation("Safe {SafeId} balance restored by {Amount}", safe.Id, payment.Amount);

                payment.Reverse(reversalEntry.Id, request.ReversedBy);
                paymentRepo.Update(payment);

                _logger.LogInformation("Payment {PaymentId} marked as reversed by {User}", payment.Id, request.ReversedBy);

                invoice.RecalculateTotals();
                invoiceRepo.Update(invoice);

                _logger.LogInformation("Invoice {InvoiceNumber} totals recalculated after payment reversal", invoice.InvoiceNumber);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Transaction committed successfully for Payment {PaymentId}", payment.Id);

                return _mapper.Map<PurchasePaymentDto>(payment);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while deleting payment {PaymentId}", request.Id);
                throw;
            }
        }
    }
}
