using Khazen.Application.Common.Interfaces;
using Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons;
using Khazen.Application.Specification.SalesModule.SalesInvoicesSpecifications;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Void
{
    internal class VoidSalesInvoiceCommandHandler(
        IUnitOfWork unitOfWork,
        INumberSequenceService numberSequenceService
    ) : IRequestHandler<VoidSalesInvoiceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INumberSequenceService _numberSequenceService = numberSequenceService;

        public async Task<bool> Handle(VoidSalesInvoiceCommand request, CancellationToken cancellationToken)
        {
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

                var reversalEntry = new JournalEntry
                {
                    EntryDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    Description = $"Reversal for Voided Invoice {salesInvoice.InvoiceNumber}",
                    JournalEntryNumber = await _numberSequenceService.GetNextNumber("JE", DateTime.UtcNow.Year, cancellationToken),
                    TransactionType = TransactionType.SalesInvoiceReversal,
                    Lines = journalEntry.Lines.Select(l => new JournalEntryLine
                    {
                        AccountId = l.AccountId,
                        Debit = l.Credit,
                        Credit = l.Debit
                    }).ToList(),
                    ReversalOfJournalEntryId = journalEntry.Id
                };

                await journalRepo.AddAsync(reversalEntry, cancellationToken);

                salesInvoice.Void(request.VoidedBy);

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
