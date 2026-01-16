using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt;
using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;
using Khazen.Application.Specification.PurchaseModule.PurchaseOrderSpecifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Create
{
    internal class CreatePurchaseReceiptHandler(
       IUnitOfWork unitOfWork,
       IMapper mapper,
       IValidator<CreatePurchaseReceiptCommand> validator,
       IPurchaseReceiptFactory receiptFactory,
       IWarehouseStockService warehouseStockService,
       IPurchaseOrderStatusService purchaseOrderStatusService,
       ILogger<CreatePurchaseReceiptHandler> logger,
       UserManager<ApplicationUser> userManager
    ) : IRequestHandler<CreatePurchaseReceiptCommand, PurchaseReceiptDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreatePurchaseReceiptCommand> _validator = validator;
        private readonly IPurchaseReceiptFactory _receiptFactory = receiptFactory;
        private readonly IWarehouseStockService _warehouseStockService = warehouseStockService;
        private readonly IPurchaseOrderStatusService _purchaseOrderStatusService = purchaseOrderStatusService;
        private readonly ILogger<CreatePurchaseReceiptHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<PurchaseReceiptDto> Handle(CreatePurchaseReceiptCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Started creating Purchase Receipt for PurchaseOrder {PurchaseOrderId}.", request.Dto.PurchaseOrderId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for Purchase Receipt creation. Errors: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var user = await _userManager.FindByNameAsync(request.CurrentUserId);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserId: {ModifiedBy}", request.CurrentUserId);
                    throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
                }
                _logger.LogDebug("Fetching Purchase Order with ID {PurchaseOrderId}", request.Dto.PurchaseOrderId);
                var order = await GetPurchaseOrderAsync(request.Dto.PurchaseOrderId, cancellationToken);

                _logger.LogDebug("Fetching Warehouse with ID {WarehouseId}", request.Dto.WarehouseId);
                var warehouse = await GetWarehouseAsync(request.Dto.WarehouseId, cancellationToken);

                _logger.LogDebug("Creating Purchase Receipt entity...");
                var receipt = _receiptFactory.CreateReceipt(order, warehouse, user.Id, request);

                _logger.LogDebug("Adjusting warehouse stock...");
                await _warehouseStockService.AdjustStockAsync(receipt, cancellationToken);

                _logger.LogDebug("Updating Purchase Order status...");
                _purchaseOrderStatusService.UpdateStatus(order, receipt);

                _logger.LogDebug("Saving Purchase Receipt...");
                await _unitOfWork.GetRepository<PurchaseReceipt, Guid>().AddAsync(receipt, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Purchase Receipt {PurchaseReceiptId} created successfully.", receipt.Id);

                return _mapper.Map<PurchaseReceiptDto>(receipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Purchase Receipt for PurchaseOrder {PurchaseOrderId}", request.Dto.PurchaseOrderId);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        private async Task<PurchaseOrder> GetPurchaseOrderAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.GetRepository<PurchaseOrder, Guid>();
            var order = await repo.GetAsync(new GetPurhaseOrderByIdSpec(purchaseOrderId), cancellationToken);

            if (order is null)
            {
                _logger.LogWarning("Purchase Order with ID {PurchaseOrderId} not found.", purchaseOrderId);
                throw new NotFoundException<PurchaseOrder>(purchaseOrderId);
            }

            return order;
        }

        private async Task<Warehouse> GetWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default)
        {
            var repo = _unitOfWork.GetRepository<Warehouse, Guid>();
            var warehouse = await repo.GetByIdAsync(warehouseId, cancellationToken);

            if (warehouse is null)
            {
                _logger.LogWarning("Warehouse with ID {WarehouseId} not found.", warehouseId);
                throw new NotFoundException<Warehouse>(warehouseId);
            }

            return warehouse;
        }
    }
}
