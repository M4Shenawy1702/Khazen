using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseInvoice;
using Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications;
using Khazen.Application.Specification.PurchaseModule.PurchaseReceiptSpecifications;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Create;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services.PurchaseModuleServices.PurchaseInvoiceServices
{
    public class PurchaseInvoiceValidationService(
        IUnitOfWork unitOfWork,
        IValidator<CreateInvoiceForReceiptCommand> validator,
        ILogger<PurchaseInvoiceValidationService> logger
    ) : IInvoiceValidationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<CreateInvoiceForReceiptCommand> _validator = validator;
        private readonly ILogger<PurchaseInvoiceValidationService> _logger = logger;

        public async Task<PurchaseReceipt> ValidateAndGetReceiptAsync(CreateInvoiceForReceiptCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Validating invoice creation request for ReceiptId={Id}", request.Dto.PurchaseReceiptId);

            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Invoice validation failed: {@Errors}", validation.Errors);
                throw new BadRequestException(validation.Errors.Select(i => i.ErrorMessage).ToList());
            }

            var receiptRepo = _unitOfWork.GetRepository<PurchaseReceipt, Guid>();
            var receipt = await receiptRepo.GetAsync(
                new GetPurchaseReceiptWithAllIncludesByIdSpec(request.Dto.PurchaseReceiptId),
                cancellationToken
            ) ?? throw new NotFoundException<PurchaseReceipt>(request.Dto.PurchaseReceiptId);

            _logger.LogDebug("Receipt {ReceiptId} loaded successfully with {Count} items.", receipt.Id, receipt.Items?.Count ?? 0);

            if (receipt.Items == null || receipt.Items.Count == 0)
                throw new BadRequestException("Cannot create invoice for a receipt without items");

            if (receipt.Invoice != null)
                throw new BadRequestException("Receipt already has an invoice");

            var invoiceRepo = _unitOfWork.GetRepository<PurchaseInvoice, Guid>();

            if (await invoiceRepo.GetAsync(new GetPurchaseInvoiceByInvoiceNumberSpec(request.Dto.InvoiceNumber), cancellationToken, true) is not null)
                throw new BadRequestException($"Invoice number {request.Dto.InvoiceNumber} already exists.");

            if (receipt.Items.Count != request.Dto.Items.Count)
                throw new BadRequestException("Invoice items count must match receipt items count");

            _logger.LogInformation("Validation passed for invoice creation. ReceiptId={ReceiptId}", receipt.Id);

            return receipt;
        }
    }

}
