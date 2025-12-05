using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseInvoice;
using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Application.Specification.PurchaseModule.PurchaseReceiptSpecifications;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Create
{
    public class CreateInvoiceForReceiptCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateInvoiceForReceiptCommand> validator,
    IInvoiceFactoryService invoiceFactory,
    IPurchaseInvoiceStockCostService stockService,
    IJournalEntryService journalService,
    ILogger<CreateInvoiceForReceiptCommandHandler> logger
) : IRequestHandler<CreateInvoiceForReceiptCommand, PurchaseInvoiceDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateInvoiceForReceiptCommand> _validator = validator;
        private readonly IInvoiceFactoryService _invoiceFactory = invoiceFactory;
        private readonly IPurchaseInvoiceStockCostService _stockService = stockService;
        private readonly IJournalEntryService _journalService = journalService;
        private readonly ILogger<CreateInvoiceForReceiptCommandHandler> _logger = logger;

        public async Task<PurchaseInvoiceDto> Handle(CreateInvoiceForReceiptCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting CreateInvoiceForReceiptCommandHandler for Receipt {ReceiptId}", request.Dto.PurchaseReceiptId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed: {@Errors}", validationResult.Errors);
                throw new BadRequestException(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var receiptRepo = _unitOfWork.GetRepository<PurchaseReceipt, Guid>();
                var receipt = await receiptRepo.GetAsync(
                    new GetPurchaseReceiptWithAllIncludesByIdSpec(request.Dto.PurchaseReceiptId),
                    cancellationToken);
                if (receipt is null)
                {
                    _logger.LogWarning("Receipt {ReceiptId} not found.", request.Dto.PurchaseReceiptId);
                    throw new NotFoundException<PurchaseReceipt>(request.Dto.PurchaseReceiptId);
                }

                if (receipt.InvoiceId is not null)
                {
                    _logger.LogWarning("Invoice already exists for Receipt {ReceiptId}.", request.Dto.PurchaseReceiptId);
                    throw new DomainException("Invoice already created for this receipt.");
                }

                var invoice = await _invoiceFactory.CreateInvoiceAsync(receipt, request, cancellationToken);

                await _stockService.UpdateStockAndCostAsync(invoice, receipt.WarehouseId, cancellationToken);

                var journalEntry = await _journalService.GeneratePurchaseJournalEntryAsync(invoice, cancellationToken);
                invoice.MarkAsPosted(journalEntry.Id);

                var invoiceRepo = _unitOfWork.GetRepository<PurchaseInvoice, Guid>();
                await invoiceRepo.AddAsync(invoice, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Invoice {InvoiceNumber} created successfully.", invoice.InvoiceNumber);
                return _mapper.Map<PurchaseInvoiceDto>(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice for Receipt {ReceiptId}. Rolling back...", request.Dto.PurchaseReceiptId);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
