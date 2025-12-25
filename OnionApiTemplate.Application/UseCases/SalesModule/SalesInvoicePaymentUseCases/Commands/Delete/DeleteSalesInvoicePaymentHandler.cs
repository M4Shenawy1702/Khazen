using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.Interfaces.ISalesModule;
using Khazen.Application.Common.Interfaces.ISalesModule.ISalesInvoicePaymentServices;
using Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos;
using Khazen.Application.Specification.SalesModule.SalesInvoicesSpecifications;
using Khazen.Domain.Entities;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Delete
{
    internal class DeleteSalesInvoicePaymentHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    INumberSequenceService numberSequenceService,
    ILogger<DeleteSalesInvoicePaymentHandler> logger, ISalesPaymentDomainService salesPaymentDomainService, IJournalEntryService journalEntryService, ISafeTransactionService safeTransactionService, UserManager<ApplicationUser> userManager
    ) : IRequestHandler<DeleteSalesInvoicePaymentCommand, SalesInvoicePaymentDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly INumberSequenceService _numberSequenceService = numberSequenceService;
        private readonly ILogger<DeleteSalesInvoicePaymentHandler> _logger = logger;
        private readonly ISalesPaymentDomainService _salesPaymentDomainService = salesPaymentDomainService;
        private readonly IJournalEntryService _journalEntryService = journalEntryService;
        private readonly ISafeTransactionService _safeTransactionService = safeTransactionService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<SalesInvoicePaymentDto> Handle(DeleteSalesInvoicePaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting reversal for SalesInvoicePayment {PaymentId}", request.Id);
            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogInformation("User not found. Username: {CurrentUserId}", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var paymentRepo = _unitOfWork.GetRepository<SalesInvoicePayment, Guid>();
                var payment = await paymentRepo.GetByIdAsync(request.Id, cancellationToken);
                if (payment == null)
                {
                    _logger.LogWarning("Payment {Id} not found", request.Id);
                    throw new NotFoundException<SalesInvoicePayment>(request.Id);
                }

                _salesPaymentDomainService.ReversePayment(request, payment);
                JournalEntry reversalJournal = await _journalEntryService.CreateReverseSalesPaymentJournalEntry(payment, user.UserName!, cancellationToken);

                await _unitOfWork.GetRepository<JournalEntry, Guid>().AddAsync(reversalJournal, cancellationToken);

                var safeRepo = _unitOfWork.GetRepository<Safe, Guid>();

                await _safeTransactionService.SalesPaymentSafeReversal(request, payment, reversalJournal, safeRepo, cancellationToken);

                var invoiceRepo = _unitOfWork.GetRepository<SalesInvoice, Guid>();
                var invoice = await invoiceRepo.GetAsync(new GetSalesInvoiceWithIncludesSpecifications(payment.SalesInvoiceId), cancellationToken);
                if (invoice == null)
                {
                    _logger.LogError("Invoice {InvoiceId} not found for reversal of payment {PaymentId}", payment.SalesInvoiceId, payment.Id);
                    throw new NotFoundException<SalesInvoice>(payment.SalesInvoiceId);
                }

                invoice.UpdateInvoiceStatus();
                invoiceRepo.Update(invoice);

                payment.ReversalJournalEntry = reversalJournal;
                paymentRepo.Update(payment);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Reversal completed successfully for payment {PaymentId}", payment.Id);

                return _mapper.Map<SalesInvoicePaymentDto>(payment);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Concurrency issue while reversing payment {Id}", request.Id);
                throw new BadRequestException("The Safe was modified by another transaction. Try again.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error reversing SalesInvoicePayment {Id}", request.Id);
                throw;
            }
        }
    }
}
