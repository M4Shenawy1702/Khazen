using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt;
using Khazen.Application.Specification.PurchaseModule.PurchaseReceiptSpecifications;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Delete
{
    internal class DeletePurchaseReceiptHandler(
        IUnitOfWork unitOfWork,
        IDeletePurchaseReceiptService deleteService,
        ILogger<DeletePurchaseReceiptHandler> logger,
        UserManager<ApplicationUser> userManager
    ) : IRequestHandler<DeletePurchaseReceiptCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IDeletePurchaseReceiptService _deleteService = deleteService;
        private readonly ILogger<DeletePurchaseReceiptHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(DeletePurchaseReceiptCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting deletion of PurchaseReceipt {PurchaseReceiptId}", request.Id);

            var receiptRepo = _unitOfWork.GetRepository<PurchaseReceipt, Guid>();
            var purchaseReceipt = await receiptRepo.GetAsync(
                new GetPurchaseReceiptWithItemsByIdSpec(request.Id), cancellationToken);

            if (purchaseReceipt is null)
            {
                _logger.LogWarning("PurchaseReceipt {PurchaseReceiptId} not found.", request.Id);
                throw new NotFoundException<PurchaseReceipt>(request.Id);
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
                await _deleteService.DeleteReceiptAsync(purchaseReceipt, user.Id, request.RowVersion, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Successfully deleted PurchaseReceipt {PurchaseReceiptId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete PurchaseReceipt {PurchaseReceiptId}. Rolling back transaction...", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
