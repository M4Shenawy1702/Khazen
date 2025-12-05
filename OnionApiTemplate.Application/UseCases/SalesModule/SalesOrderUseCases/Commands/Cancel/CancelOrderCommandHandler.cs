using Khazen.Application.Common.Interfaces.SalesModule.ISalesOrderModule;
using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;
using Khazen.Application.Specification.InventoryModule.WareHouseProductSpecifications;
using Khazen.Application.Specification.SalesModule.SalesOrdersSpecifications;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Cancel
{
    internal class CancelOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CancelOrderCommand> validator,
        IStockReservationService stockReservationService,
        ILogger<CancelOrderCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<CancelOrderCommand, SalesOrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CancelOrderCommand> _validator = validator;
        private readonly IStockReservationService _stockReservationService = stockReservationService;
        private readonly ILogger<CancelOrderCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<SalesOrderDto> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting CancelOrder process | OrderId: {OrderId} | CanceledBy: {CanceledBy}",
                request.Id, request.CanceledBy);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "CancelOrder validation failed | OrderId: {OrderId} | Errors: {Errors}",
                    request.Id,
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                throw new BadRequestException(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _logger.LogInformation("Transaction started for cancel order {OrderId}", request.Id);

            try
            {
                var user = await _userManager.FindByNameAsync(request.CanceledBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserName: {CanceledBy}", request.CanceledBy);
                    throw new NotFoundException<ApplicationUser>(request.CanceledBy);
                }

                var orderRepo = _unitOfWork.GetRepository<SalesOrder, Guid>();
                var salesOrder = await orderRepo.GetAsync(new GetSalesOrderByIdSpecification(request.Id), cancellationToken);
                if (salesOrder is null)
                {
                    _logger.LogWarning("Sales order not found | OrderId: {OrderId}", request.Id);
                    throw new NotFoundException<SalesOrder>(request.Id);
                }

                if (request.RowVersion is null)
                {
                    _logger.LogWarning("RowVersion missing for order {OrderId}", request.Id);
                    throw new BadRequestException("RowVersion is required.");
                }

                _logger.LogInformation(
                    "Sales order loaded successfully | OrderId: {OrderId} | Status: {Status}",
                    salesOrder.Id, salesOrder.Status);

                salesOrder.AssertRowVersion(request.RowVersion);
                _logger.LogInformation("RowVersion validated for OrderId {OrderId}", request.Id);

                if (salesOrder.Status == OrderStatus.Cancelled)
                {
                    _logger.LogWarning("Attempt to cancel an already cancelled order. OrderId {OrderId}", request.Id);
                    throw new BadRequestException("Order already cancelled.");
                }

                if (salesOrder.Status is OrderStatus.Shipped or OrderStatus.Delivered)
                {
                    _logger.LogWarning(
                        "Attempt to cancel order in invalid state | OrderId: {OrderId} | Status: {Status}",
                        request.Id, salesOrder.Status);

                    throw new BadRequestException($"Order with status {salesOrder.Status} cannot be cancelled.");
                }

                _logger.LogInformation("Loading warehouse product reservations for OrderId {OrderId}", request.Id);

                var wpRepo = _unitOfWork.GetRepository<WarehouseProduct, int>();
                var productIds = salesOrder.Items.Select(i => i.ProductId).ToList();

                var warehouseProducts = await wpRepo.GetAllAsync(new GetWarehouseProductsSpecification(productIds), cancellationToken);

                _logger.LogInformation(
                    "Warehouse products loaded for unreserve | OrderId: {OrderId} | ProductCount: {Count}",
                    request.Id, warehouseProducts.Count());

                await _stockReservationService.UnReserveStockAsync(salesOrder, warehouseProducts);
                wpRepo.UpdateRange(warehouseProducts);

                _logger.LogInformation("Stock unreserved successfully for OrderId {OrderId}", request.Id);

                salesOrder.MarkAsCanceled(request.CanceledBy);

                _logger.LogInformation(
                    "Order marked as cancelled | OrderId: {OrderId} | CanceledBy: {CanceledBy}",
                    request.Id, request.CanceledBy);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Transaction committed successfully | OrderId: {OrderId}", request.Id);

                return _mapper.Map<SalesOrderDto>(salesOrder);
            }
            catch (ConcurrencyException ex)
            {
                _logger.LogError(
                    ex,
                    "Concurrency conflict while cancelling order | OrderId: {OrderId}",
                    request.Id);

                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while cancelling order | OrderId: {OrderId}",
                    request.Id);

                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
