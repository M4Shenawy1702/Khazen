using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Delete
{
    internal class ToggleSupplierCommandHandler(IUnitOfWork unitOfWork, ILogger<ToggleSupplierCommandHandler> logger)
    : IRequestHandler<ToggleSupplierCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ToggleSupplierCommandHandler> _logger = logger;

        public async Task<bool> Handle(ToggleSupplierCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var repo = _unitOfWork.GetRepository<Supplier, Guid>();
                var supplier = await repo.GetByIdAsync(request.Id, cancellationToken);
                if (supplier is null)
                {
                    _logger.LogWarning("Supplier not found. Id: {SupplierId}", request.Id);
                    throw new NotFoundException<Supplier>(request.Id);
                }

                _logger.LogInformation("Toggling supplier. Id: {SupplierId}, CurrentUserId: {User}",
                    supplier.Id, request.CurrentUserId);

                supplier.Toggle(request.CurrentUserId);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Supplier toggled successfully. Id: {SupplierId}, Active: {IsActive}",
                    supplier.Id, supplier.IsActive);

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error toggling supplier. Id: {SupplierId}", request.Id);
                throw;
            }
        }
    }
}
