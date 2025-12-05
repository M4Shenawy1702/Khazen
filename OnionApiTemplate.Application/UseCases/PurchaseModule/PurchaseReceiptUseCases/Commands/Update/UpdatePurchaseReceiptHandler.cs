using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt;
using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;
using Khazen.Application.Specification.PurchaseModule.PurchaseOrderSpecifications;
using Khazen.Application.Specification.PurchaseModule.PurchaseReceiptSpecifications;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Update;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

public class UpdatePurchaseReceiptHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdatePurchaseReceiptCommand> validator,
    IPurchaseReceiptUpdateService receiptUpdateService, ILogger<UpdatePurchaseReceiptHandler> logger
) : IRequestHandler<UpdatePurchaseReceiptCommand, PurchaseReceiptDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<UpdatePurchaseReceiptCommand> _validator = validator;
    private readonly IPurchaseReceiptUpdateService _receiptUpdateService = receiptUpdateService;
    private readonly ILogger<UpdatePurchaseReceiptHandler> _logger = logger;

    public async Task<PurchaseReceiptDto> Handle(UpdatePurchaseReceiptCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for updating Purchase Receipt ID {PurchaseReceiptId}: {Errors}",
                request.Id, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var receiptRepo = _unitOfWork.GetRepository<PurchaseReceipt, Guid>();
            var purchaseReceipt = await receiptRepo.GetAsync(new GetPurchaseReceiptWithItemsByIdSpec(request.Id), cancellationToken);
            if (purchaseReceipt is null)
            {
                _logger.LogError("Purchase Receipt with ID {PurchaseReceiptId} not found", request.Id);
                throw new NotFoundException<PurchaseReceipt>(request.Id);
            }

            if (purchaseReceipt.IsDeleted)
            {
                _logger.LogError("Purchase Receipt with ID {PurchaseReceiptId} already deleted", request.Id);
                throw new BadRequestException($"Purchase Receipt with ID {purchaseReceipt.Id} already deleted");
            }

            var orderRepo = _unitOfWork.GetRepository<PurchaseOrder, Guid>();
            var purchaseOrder = await orderRepo.GetAsync(new GetPurhaseOrderByIdSpec(purchaseReceipt.PurchaseOrderId), cancellationToken);
            if (purchaseOrder is null)
            {
                _logger.LogError("Purchase Order with ID {PurchaseOrderId} not found for Purchase Receipt ID {PurchaseReceiptId}", purchaseReceipt.PurchaseOrderId, purchaseReceipt.Id);
                throw new NotFoundException<PurchaseOrder>(purchaseReceipt.PurchaseOrderId);
            }

            if (purchaseOrder.Status == PurchaseOrderStatus.FullyReceived)
            {
                _logger.LogError("Purchase Order with ID {PurchaseOrderId} is already fully received for Purchase Receipt ID {PurchaseReceiptId}", purchaseReceipt.PurchaseOrderId, purchaseReceipt.Id);
                throw new BadRequestException($"Purchase Order {purchaseOrder.Id} is already fully received, cannot update receipt.");
            }

            _logger.LogInformation("Starting update for Purchase Receipt ID {PurchaseReceiptId}", purchaseReceipt.Id);

            await _receiptUpdateService.UpdateReceiptAsync(request, purchaseReceipt, purchaseOrder, cancellationToken);
            _logger.LogInformation("Purchase Receipt ID {PurchaseReceiptId} updated successfully", purchaseReceipt.Id);

            receiptRepo.Update(purchaseReceipt);
            orderRepo.Update(purchaseOrder);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Transaction committed for Purchase Receipt ID {PurchaseReceiptId}", purchaseReceipt.Id);

            return _mapper.Map<PurchaseReceiptDto>(purchaseReceipt);
        }
        catch (ConcurrencyException ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error updating Purchase Receipt ID {PurchaseReceiptId}, transaction rolled back", request.Id);
            throw;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error updating Purchase Receipt ID {PurchaseReceiptId}, transaction rolled back", request.Id);
            throw;
        }
    }
}
