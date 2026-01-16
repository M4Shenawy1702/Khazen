using Khazen.Application.Common.Interfaces;
using Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Reverse
{
    public class ReversePurchaseInvoiceCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<ReversePurchaseInvoiceCommandHandler> logger,
        IJournalEntryService journalEntryService,
        UserManager<ApplicationUser> userManager
    ) : IRequestHandler<ReversePurchaseInvoiceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ReversePurchaseInvoiceCommandHandler> _logger = logger;
        private readonly IJournalEntryService _journalEntryService = journalEntryService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(ReversePurchaseInvoiceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ReverseInvoice: Initiating reversal for Invoice {Id} by User {UserId}",
                request.Id, request.CurrentUserId);

            var userTask = _userManager.FindByIdAsync(request.CurrentUserId);
            var repo = _unitOfWork.GetRepository<PurchaseInvoice, Guid>();
            var invoiceTask = repo.GetAsync(new GetPurchaseInvoiceWithAllIncludesByIdSpec(request.Id), cancellationToken);

            await Task.WhenAll(userTask, invoiceTask);

            var user = await userTask ?? throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            var invoice = await invoiceTask ?? throw new NotFoundException<PurchaseInvoice>(request.Id);

            if (invoice.PaymentStatus != PaymentStatus.Unpaid)
            {
                _logger.LogWarning("ReverseInvoice: Blocked. Invoice {InvoiceNo} is already {Status}.",
                    invoice.InvoiceNumber, invoice.PaymentStatus);
                throw new BadRequestException("Cannot reverse an invoice that is partially or fully paid. Void the payments first.");
            }

            if (invoice.IsReversed)
            {
                throw new BadRequestException("This invoice has already been reversed.");
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                _logger.LogDebug("ReverseInvoice: Generating counter-journal entry.");
                var reversalEntry = await _journalEntryService.GenerateReversalPurchaseInvoiceAsync(invoice, user.Id, cancellationToken);

                var journalEntryRepo = _unitOfWork.GetRepository<JournalEntry, Guid>();
                await journalEntryRepo.AddAsync(reversalEntry, cancellationToken);

                invoice.Reverse(reversalEntry.Id, user.Id);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("ReverseInvoice: SUCCESS. Invoice {InvoiceNo} reversed by Journal {JournalNo}",
                    invoice.InvoiceNumber, reversalEntry.JournalEntryNumber);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "ReverseInvoice: Failure. Rolling back reversal for Invoice {Id}", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
