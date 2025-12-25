using Khazen.Application.Common.Interfaces;
using Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons;
using Khazen.Application.Specification.SalesModule.SalesInvoicesSpecifications;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Void
{
    internal class VoidSalesInvoiceCommandHandler(
        IUnitOfWork unitOfWork,
        INumberSequenceService numberSequenceService, ILogger<VoidSalesInvoiceCommandHandler> logger, UserManager<ApplicationUser> userManager
    ) : IRequestHandler<VoidSalesInvoiceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INumberSequenceService _numberSequenceService = numberSequenceService;
        private readonly ILogger<VoidSalesInvoiceCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(VoidSalesInvoiceCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogInformation("User not found. Username: {CurrentUserId}", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }
            await _unitOfWork.BeginTransactionAsync(cancellationToken);


            try
            {
                var invoiceRepo = _unitOfWork.GetRepository<SalesInvoice, Guid>();
                var journalRepo = _unitOfWork.GetRepository<JournalEntry, Guid>();

                var salesInvoice = await invoiceRepo.GetAsync(new GetSalesInvoiceWithIncludesSpecifications(request.Id), cancellationToken)
                     ?? throw new NotFoundException<SalesInvoice>(request.Id);

                if (salesInvoice.IsVoided)
                    throw new BadRequestException("Invoice is already voided.");

                if (salesInvoice.Payments.Any(p => !p.IsReversed))
                    throw new BadRequestException("Cannot void invoice with associated payments or credit notes.");


                var journalEntry = await journalRepo.GetAsync(new GetJurnalEntryByIdWithIncludesSpecification(salesInvoice.JournalEntryId), cancellationToken)
                    ?? throw new NotFoundException<JournalEntry>(salesInvoice.JournalEntryId);

                if (journalEntry == null)
                    throw new BadRequestException("Original journal entry is missing.");

                var reversalEntry = journalEntry.CreateReversal(user.UserName!, $"Reversed JE for JE-Number : {journalEntry.JournalEntryNumber}");

                reversalEntry.JournalEntryNumber = await _numberSequenceService.GetNextNumber(
                    "JE",
                    DateTime.UtcNow.Year,
                    cancellationToken
                );

                _logger.LogInformation(
                    "Created reversal journal entry {ReversalNumber} for Sales Invoice {InvoiceNumber}",
                    reversalEntry.JournalEntryNumber,
                    salesInvoice.InvoiceNumber
                );


                await journalRepo.AddAsync(reversalEntry, cancellationToken);

                salesInvoice.Void(user.UserName!);

                invoiceRepo.Update(salesInvoice);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
