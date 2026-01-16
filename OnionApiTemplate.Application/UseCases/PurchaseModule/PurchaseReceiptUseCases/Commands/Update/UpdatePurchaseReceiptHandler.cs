using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt;
using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;
using Khazen.Application.Specification.PurchaseModule.PurchaseOrderSpecifications;
using Khazen.Application.Specification.PurchaseModule.PurchaseReceiptSpecifications;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Update
{
    public class UpdatePurchaseReceiptHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdatePurchaseReceiptCommand> validator,
        IPurchaseReceiptUpdateService receiptUpdateService,
        ILogger<UpdatePurchaseReceiptHandler> logger,
        UserManager<ApplicationUser> userManager
    ) : IRequestHandler<UpdatePurchaseReceiptCommand, PurchaseReceiptDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdatePurchaseReceiptCommand> _validator = validator;
        private readonly IPurchaseReceiptUpdateService _receiptUpdateService = receiptUpdateService;
        private readonly ILogger<UpdatePurchaseReceiptHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<PurchaseReceiptDto> Handle(UpdatePurchaseReceiptCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for updating Purchase Receipt ID {PurchaseReceiptId}: {Errors}",
                    request.Id, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogInformation("User not found. UserId: {ModifiedBy}", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var receiptRepo = _unitOfWork.GetRepository<PurchaseReceipt, Guid>();
                var purchaseReceipt = await receiptRepo.GetAsync(
                    new GetPurchaseReceiptWithItemsByIdSpec(request.Id),
                    cancellationToken);

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
                var purchaseOrder = await orderRepo.GetAsync(
                    new GetPurhaseOrderByIdSpec(purchaseReceipt.PurchaseOrderId),
                    cancellationToken);

                if (purchaseOrder is null)
                {
                    _logger.LogError("Purchase Order with ID {PurchaseOrderId} not found for Purchase Receipt ID {PurchaseReceiptId}", purchaseReceipt.PurchaseOrderId, purchaseReceipt.Id);
                    throw new NotFoundException<PurchaseOrder>(purchaseReceipt.PurchaseOrderId);
                }

                if (purchaseOrder.Status == PurchaseOrderStatus.FullyReceived)
                {
                    _logger.LogError("Purchase Order with ID {PurchaseOrderId} is already fully received for Purchase Receipt ID {PurchaseReceiptId}", purchaseOrder.Id, purchaseReceipt.Id);
                    throw new BadRequestException($"Purchase Order {purchaseOrder.Id} is already fully received, cannot update receipt.");
                }

                _logger.LogInformation("Starting update for Purchase Receipt ID {PurchaseReceiptId}", purchaseReceipt.Id);

                _unitOfWork.SetOriginalRowVersion(purchaseReceipt, request.Dto.RowVersion);
                _unitOfWork.SetOriginalRowVersion(purchaseOrder, request.Dto.OrderRowVersion);

                await _receiptUpdateService.UpdateReceiptAsync(request, purchaseReceipt, purchaseOrder, cancellationToken);

                purchaseReceipt.ModifiedBy = user.Id;
                purchaseReceipt.ModifiedAt = DateTime.UtcNow;

                receiptRepo.Update(purchaseReceipt);
                orderRepo.Update(purchaseOrder);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Purchase Receipt ID {PurchaseReceiptId} updated successfully and transaction committed", purchaseReceipt.Id);

                return _mapper.Map<PurchaseReceiptDto>(purchaseReceipt);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning(ex, "Concurrency conflict while updating Purchase Receipt ID {PurchaseReceiptId}, transaction rolled back", request.Id);
                throw new ConflictException("The purchase receipt or related purchase order was modified by another user. Please refresh and try again.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Unexpected error updating Purchase Receipt ID {PurchaseReceiptId}, transaction rolled back", request.Id);
                throw;
            }
        }
    }
}