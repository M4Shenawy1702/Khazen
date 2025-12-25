using Khazen.Application.Common.Interfaces;
using Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Delete
{
    public class DeletePurchaseInvoiceCommandHandler(
        IUnitOfWork unitOfWork,
        INumberSequenceService numberSequenceService,
        IGetSystemValues getSystemValues,
        ILogger<DeletePurchaseInvoiceCommandHandler> logger,
        IJournalEntryService journalEntryService,
        UserManager<ApplicationUser> userManager
    ) : IRequestHandler<DeletePurchaseInvoiceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INumberSequenceService _numberSequenceService = numberSequenceService;
        private readonly IGetSystemValues _getSystemValues = getSystemValues;
        private readonly ILogger<DeletePurchaseInvoiceCommandHandler> _logger = logger;
        private readonly IJournalEntryService _journalEntryService = journalEntryService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(DeletePurchaseInvoiceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting DeletePurchaseInvoiceCommandHandler for InvoiceId: {InvoiceId}", request.Id);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Transaction started.");

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogInformation("User not found. Username: {CurrentUserId}", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }
            try
            {
                var repo = _unitOfWork.GetRepository<PurchaseInvoice, Guid>();
                var invoice = await repo.GetAsync(new GetPurchaseInvoiceWithAllIncludesByIdSpec(request.Id), cancellationToken);
                if (invoice is null)
                {
                    _logger.LogWarning("Invoice {InvoiceId} not found.", request.Id);
                    throw new NotFoundException<PurchaseInvoice>(request.Id);
                }
                _logger.LogDebug("Loaded Invoice {InvoiceNo} with PaymentStatus={PaymentStatus}", invoice.InvoiceNumber, invoice.PaymentStatus);

                if (invoice.PaymentStatus != PaymentStatus.Unpaid)
                {
                    _logger.LogWarning("Cannot delete Invoice {InvoiceNo} because it has payments.", invoice.InvoiceNumber);
                    throw new BadRequestException("Cannot delete an invoice that has payments.");
                }

                var reversalEntry = await _journalEntryService.GenerateReversalPurchaseInvoiceAsync(invoice, user.UserName!, cancellationToken);

                var journalEntryRepo = _unitOfWork.GetRepository<JournalEntry, Guid>();
                await journalEntryRepo.AddAsync(reversalEntry, cancellationToken);
                _logger.LogInformation("Reversal JournalEntry {JournalNo} created for Invoice {InvoiceNo}", reversalEntry.JournalEntryNumber, invoice.InvoiceNumber);

                invoice.Reverse(reversalEntry.Id);

                _logger.LogInformation("Invoice {InvoiceNo} marked for deletion.", invoice.InvoiceNumber);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Transaction committed successfully for Invoice {InvoiceNo}.", invoice.InvoiceNumber);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Invoice {InvoiceId}, rolling back transaction.", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
