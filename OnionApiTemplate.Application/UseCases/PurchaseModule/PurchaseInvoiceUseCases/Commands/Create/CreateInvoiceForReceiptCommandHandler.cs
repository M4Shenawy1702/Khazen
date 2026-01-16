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
    ILogger<CreateInvoiceForReceiptCommandHandler> logger,
    UserManager<ApplicationUser> userManager)
        : IRequestHandler<CreateInvoiceForReceiptCommand, PurchaseInvoiceDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateInvoiceForReceiptCommand> _validator = validator;
        private readonly IInvoiceFactoryService _invoiceFactory = invoiceFactory;
        private readonly IPurchaseInvoiceStockCostService _stockService = stockService;
        private readonly IJournalEntryService _journalService = journalService;
        private readonly ILogger<CreateInvoiceForReceiptCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<PurchaseInvoiceDto> Handle(CreateInvoiceForReceiptCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateInvoice: Initiating invoice for Receipt {ReceiptId} by User {UserId}",
                request.Dto.PurchaseReceiptId, request.CurrentUserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("CreateInvoice: Validation failed for Receipt {ReceiptId}", request.Dto.PurchaseReceiptId);
                throw new BadRequestException(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            var userTask = _userManager.FindByIdAsync(request.CurrentUserId);
            var receiptRepo = _unitOfWork.GetRepository<PurchaseReceipt, Guid>();
            var receiptTask = receiptRepo.GetAsync(new GetPurchaseReceiptWithAllIncludesByIdSpec(request.Dto.PurchaseReceiptId), cancellationToken);

            await Task.WhenAll(userTask, receiptTask);

            var user = await userTask ?? throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            var receipt = await receiptTask ?? throw new NotFoundException<PurchaseReceipt>(request.Dto.PurchaseReceiptId);

            if (receipt.InvoiceId is not null)
            {
                _logger.LogWarning("CreateInvoice: Conflict. Receipt {ReceiptId} already has Invoice {InvoiceId}",
                    receipt.Id, receipt.InvoiceId);
                throw new ConflictException("An invoice has already been generated for this receipt.");
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                _logger.LogDebug("CreateInvoice: Generating Invoice object via Factory.");
                var invoice = await _invoiceFactory.CreateInvoiceAsync(receipt, request.Dto, user.Id, cancellationToken);

                _logger.LogDebug("CreateInvoice: Recalculating Stock Costs for Warehouse {WarehouseId}", receipt.WarehouseId);
                await _stockService.UpdateStockAndCostAsync(invoice, receipt.WarehouseId, cancellationToken);

                _logger.LogDebug("CreateInvoice: Generating Financial Journal Entries.");
                var journalEntry = await _journalService.GeneratePurchaseJournalEntryAsync(invoice, user.Id, cancellationToken);

                invoice.MarkAsPosted(journalEntry.Id);

                var invoiceRepo = _unitOfWork.GetRepository<PurchaseInvoice, Guid>();
                await invoiceRepo.AddAsync(invoice, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("CreateInvoice: SUCCESS. Invoice {InvoiceNo} posted. Total: {Total}. Journal: {JournalId}",
                    invoice.InvoiceNumber, invoice.TotalAmount, journalEntry.Id);

                return _mapper.Map<PurchaseInvoiceDto>(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "CreateInvoice: FATAL ERROR for Receipt {ReceiptId}. Rolling back transaction.", request.Dto.PurchaseReceiptId);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
