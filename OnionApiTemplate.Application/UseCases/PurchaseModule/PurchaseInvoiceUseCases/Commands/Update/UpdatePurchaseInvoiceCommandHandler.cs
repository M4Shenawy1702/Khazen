using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications;
using Khazen.Application.Specification.PurchaseModule.PurchaseReceiptSpecifications;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Update
{
    public class UpdatePurchaseInvoiceCommandHandler
        : IRequestHandler<UpdatePurchaseInvoiceCommand, PurchaseInvoiceDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdatePurchaseInvoiceCommand> _validator;
        private readonly ILogger<UpdatePurchaseInvoiceCommandHandler> _logger;

        public UpdatePurchaseInvoiceCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<UpdatePurchaseInvoiceCommand> validator,
            ILogger<UpdatePurchaseInvoiceCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
            _logger = logger;
        }

        public async Task<PurchaseInvoiceDto> Handle(UpdatePurchaseInvoiceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting UpdatePurchaseInvoice | InvoiceId={InvoiceId} | ModifiedBy={User}",
                request.Id, request.ModifiedBy);

            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));

                _logger.LogWarning(
                    "Validation failed | InvoiceId={InvoiceId} | Errors={Errors}",
                    request.Id, errors);

                throw new BadRequestException(validation.Errors.Select(e => e.ErrorMessage).ToList());
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                _logger.LogDebug(
                    "Fetching PurchaseInvoice with includes | InvoiceId={InvoiceId}",
                    request.Id);

                var invoiceRepo = _unitOfWork.GetRepository<PurchaseInvoice, Guid>();
                var invoice = await invoiceRepo.GetAsync(
                    new GetPurchaseInvoiceWithAllIncludesByIdSpec(request.Id),
                    cancellationToken);

                if (invoice == null)
                {
                    _logger.LogWarning("PurchaseInvoice not found | InvoiceId={InvoiceId}", request.Id);
                    throw new NotFoundException<PurchaseInvoice>(request.Id);
                }

                _logger.LogDebug(
                    "Fetching related PurchaseReceipt | ReceiptId={ReceiptId}",
                    invoice.PurchaseReceiptId);

                var receiptRepo = _unitOfWork.GetRepository<PurchaseReceipt, Guid>();
                var receipt = await receiptRepo.GetAsync(
                    new GetPurchaseReceiptWithAllIncludesByIdSpec(invoice.PurchaseReceiptId),
                    cancellationToken);

                if (receipt == null)
                {
                    _logger.LogWarning(
                        "Related PurchaseReceipt not found | ReceiptId={ReceiptId}",
                        invoice.PurchaseReceiptId);

                    throw new NotFoundException<PurchaseReceipt>(invoice.PurchaseReceiptId);
                }

                if (invoice.InvoiceStatus != InvoiceStatus.Draft)
                {
                    _logger.LogWarning(
                        "Attempt to update non-draft invoice | InvoiceId={InvoiceId} | Status={Status}",
                        invoice.Id, invoice.InvoiceStatus);

                    throw new BadRequestException("Only pending invoices can be updated.");
                }

                if (invoice.IsPosted)
                {
                    _logger.LogWarning(
                        "Attempt to update posted invoice | InvoiceId={InvoiceId}",
                        invoice.Id);

                    throw new BadRequestException("Cannot update a posted invoice. Reverse it instead.");
                }

                if (invoice.IsReversed)
                {
                    _logger.LogWarning(
                        "Attempt to update reversed invoice | InvoiceId={InvoiceId}",
                        invoice.Id);

                    throw new BadRequestException("Cannot update a reversed invoice.");
                }

                _logger.LogInformation(
                    "Updating invoice values | InvoiceId={InvoiceId} | NewNumber={NewNumber}",
                    invoice.Id, request.Dto.InvoiceNumber);

                invoice.Modify(
                    request.Dto.InvoiceNumber,
                    request.ModifiedBy,
                    request.Dto.Notes);

                _logger.LogInformation(
                    "Invoice updated | InvoiceId={InvoiceId} | ModifiedBy={User}",
                    invoice.Id, request.ModifiedBy);

                _logger.LogDebug("Recalculating totals | InvoiceId={InvoiceId}", invoice.Id);
                invoice.RecalculateTotals();

                _logger.LogDebug(
                    "Updating related PurchaseReceipt | ReceiptId={ReceiptId}",
                    invoice.PurchaseReceiptId);

                _logger.LogDebug("Committing transaction | InvoiceId={InvoiceId}", invoice.Id);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation(
                    "PurchaseInvoice update completed successfully | InvoiceId={InvoiceId}",
                    invoice.Id);

                return _mapper.Map<PurchaseInvoiceDto>(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error updating PurchaseInvoice | InvoiceId={InvoiceId} | Rolling back transaction...",
                    request.Id);

                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
