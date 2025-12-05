using Khazen.Application.Common.Interfaces.SalesModule.ISalesOrderModule;
using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;
using Khazen.Application.Specification.InventoryModule.WareHouseProductSpecifications;
using Khazen.Application.Specification.SalesModule.SalesOrdersSpecifications;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Ship
{
    internal class ShipOrderCommandHandler(
    IUnitOfWork unitOfWork,
    IValidator<ShipOrderCommand> validator,
    IMapper mapper,
    IStockReservationService stockReservationService,
    UserManager<ApplicationUser> userManager,
    ILogger<ShipOrderCommandHandler> logger)
    : IRequestHandler<ShipOrderCommand, SalesOrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<ShipOrderCommand> _validator = validator;
        private readonly IMapper _mapper = mapper;
        private readonly IStockReservationService _stockReservationService = stockReservationService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<ShipOrderCommandHandler> _logger = logger;

        public async Task<SalesOrderDto> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start shipping process for order {OrderId} by user {User}", request.Id, request.ShippedBy);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for order {OrderId}: {Errors}", request.Id, validationResult.Errors.Select(e => e.ErrorMessage));
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var user = await _userManager.FindByNameAsync(request.ShippedBy);
                if (user is null)
                {
                    _logger.LogWarning("User {User} not found while shipping order {OrderId}", request.ShippedBy, request.Id);
                    throw new NotFoundException<ApplicationUser>(request.ShippedBy);
                }
                _logger.LogInformation("User {User} verified for shipping order {OrderId}", request.ShippedBy, request.Id);

                var salesOrdersRepository = _unitOfWork.GetRepository<SalesOrder, Guid>();
                var salesOrder = await salesOrdersRepository.GetAsync(new GetSalesOrderByIdSpecification(request.Id), cancellationToken);
                if (salesOrder is null)
                {
                    _logger.LogWarning("Order {OrderId} not found while shipping", request.Id);
                    throw new NotFoundException<SalesOrder>(request.Id);
                }

                _logger.LogInformation("Loaded order {OrderId} with status {Status}", salesOrder.Id, salesOrder.Status);

                if (request.RowVersion is null)
                {
                    _logger.LogWarning("RowVersion missing for order {OrderId}", request.Id);
                    throw new BadRequestException("RowVersion is required.");
                }

                salesOrder.AssertRowVersion(request.RowVersion);
                _logger.LogInformation("RowVersion check passed for order {OrderId}", request.Id);

                if (salesOrder.Status != OrderStatus.Confirmed)
                {
                    _logger.LogWarning("Order {OrderId} status {Status} invalid for shipping", request.Id, salesOrder.Status);
                    throw new BadRequestException($"Only confirmed orders can be shipped. Current status: {salesOrder.Status}");
                }

                var warehouseProductsRepository = _unitOfWork.GetRepository<WarehouseProduct, int>();
                var productIds = salesOrder.Items.Select(i => i.ProductId).ToList();

                var warehouseProducts = await warehouseProductsRepository.GetAllAsync(new GetWarehouseProductsSpecification(productIds), cancellationToken);

                _logger.LogInformation("Updating reserved quantities for order {OrderId}", request.Id);
                await _stockReservationService.UpdateReservedQuantitiesWhenOrderShipped(salesOrder, warehouseProducts);

                foreach (var wp in warehouseProducts)
                {
                    _logger.LogInformation("Warehouse {WarehouseId} updated for product {ProductId}: Reserved={Reserved}, InStock={Stock}",
                        wp.WarehouseId, wp.ProductId, wp.ReservedQuantity, wp.QuantityInStock);
                }

                warehouseProductsRepository.UpdateRange(warehouseProducts);

                salesOrder.MarkAsShipped(request.ShippedBy, request.Dto.TrackingNumber);
                _logger.LogInformation("Order {OrderId} marked as shipped by {User}", salesOrder.Id, request.ShippedBy);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Shipping transaction committed successfully for order {OrderId}", request.Id);

                return _mapper.Map<SalesOrderDto>(salesOrder);
            }
            catch (ConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict while shipping order {OrderId}", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while shipping order {OrderId}", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
