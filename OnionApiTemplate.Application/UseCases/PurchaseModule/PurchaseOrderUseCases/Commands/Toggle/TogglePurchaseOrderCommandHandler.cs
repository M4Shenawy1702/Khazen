using Khazen.Application.Specification.PurchaseModule.PurchaseOrderSpecifications;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Delete
{
    internal class TogglePurchaseOrderCommandHandler(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, ILogger<TogglePurchaseOrderCommandHandler> logger)
                : IRequestHandler<TogglePurchaseOrderCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<TogglePurchaseOrderCommandHandler> _logger = logger;

        public async Task<bool> Handle(TogglePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("TogglePO: Processing toggle for ID {Id} by User {User}", request.Id, request.CurrentUserId);

            var userTask = _userManager.FindByNameAsync(request.CurrentUserId);
            var repository = _unitOfWork.GetRepository<PurchaseOrder, Guid>();
            var purchaseOrderTask = repository.GetAsync(new GetPurhaseOrderByIdSpec(request.Id), cancellationToken);

            await Task.WhenAll(userTask, purchaseOrderTask);

            var user = await userTask;
            if (user == null)
            {
                _logger.LogWarning("TogglePO: User {User} not found", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }
            var purchaseOrder = await purchaseOrderTask;
            if (purchaseOrder == null)
            {
                _logger.LogWarning("TogglePO: PO {Id} not found", request.Id);
                throw new NotFoundException<PurchaseOrder>(request.Id);
            }

            if (purchaseOrder.Status != PurchaseOrderStatus.Cancelled)
            {
                var hasReceipts = await _unitOfWork.GetRepository<PurchaseReceipt, Guid>()
                    .AnyAsync(r => r.PurchaseOrderId == request.Id, cancellationToken);

                if (hasReceipts)
                {
                    _logger.LogWarning("TogglePO: Blocked. PO {Id} has associated receipts.", request.Id);
                    throw new DomainException("Cannot cancel an order that has already been received. Reverse the receipts first.");
                }
            }
            else
            {
                var duplicateExists = await repository.AnyAsync(po =>
                    po.OrderNumber == purchaseOrder.OrderNumber &&
                    po.Status != PurchaseOrderStatus.Cancelled, cancellationToken);

                if (duplicateExists)
                    throw new DomainException("Cannot reactivate: OrderNumber is already in use by another active order.");
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                purchaseOrder.SetRowVersion(request.RowVersion);
                purchaseOrder.Toggle(user.Id);

                _logger.LogDebug("TogglePO: Saving state change to DB for PO {OrderNo}", purchaseOrder.OrderNumber);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("TogglePO: SUCCESS. New status: {Status}", purchaseOrder.Status);
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning("TogglePO: Concurrency conflict for PO {Id}", request.Id);
                throw new ConcurrencyException("The order was modified by someone else. Please refresh.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TogglePO: Critical failure for PO {Id}", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }

}
