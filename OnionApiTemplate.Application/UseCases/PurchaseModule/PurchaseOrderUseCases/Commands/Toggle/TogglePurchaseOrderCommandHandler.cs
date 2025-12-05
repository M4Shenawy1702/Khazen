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
            if (string.IsNullOrWhiteSpace(request.ModifiedBy))
                throw new BadRequestException("ModifiedBy is required.");

            _logger.LogInformation("Starting TogglePurchaseOrderCommandHandler for PurchaseOrder Id: {PurchaseOrderId}", request.Id);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var user = await _userManager.FindByNameAsync(request.ModifiedBy);
                if (user is null)
                {
                    _logger.LogWarning("User not found. UserName: {ModifiedBy}", request.ModifiedBy);
                    throw new NotFoundException<ApplicationUser>(request.ModifiedBy);
                }

                var repository = _unitOfWork.GetRepository<PurchaseOrder, Guid>();
                var purchaseOrder = await repository.GetByIdAsync(request.Id, cancellationToken);
                if (purchaseOrder is null)
                {
                    _logger.LogWarning("PurchaseOrder not found. Id: {PurchaseOrderId}", request.Id);
                    throw new NotFoundException<PurchaseOrder>(request.Id);
                }

                purchaseOrder.SetRowVersion(request.RowVersion);

                if (purchaseOrder.Status == PurchaseOrderStatus.Cancelled)
                {
                    var duplicateExists = await repository.AnyAsync(po => po.OrderNumber == purchaseOrder.OrderNumber && po.Status != PurchaseOrderStatus.Cancelled, cancellationToken);
                    if (duplicateExists)
                        throw new DomainException("Cannot reactivate order because OrderNumber is already used.");
                }

                purchaseOrder.Toggle(request.ModifiedBy);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("TogglePurchaseOrderCommandHandler completed successfully for PurchaseOrder Id: {PurchaseOrderId}", request.Id);
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException("The purchase order was updated by another user. Please reload and try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TogglePurchaseOrderCommandHandler for PurchaseOrder Id: {PurchaseOrderId}", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

    }

}
