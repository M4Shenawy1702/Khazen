using Khazen.Application.Common.Interfaces.SalesModule.ISalesOrderModule;
using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;
using Khazen.Application.Specification.InventoryModule.WareHouseProductSpecifications;
using Khazen.Application.Specification.SalesModule.CustomerSpecifications;
using Khazen.Application.Specification.SalesModule.SalesOrdersSpecifications;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Update
{
    internal class UpdateSalesOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateSalesOrderCommand> validator,
        ILogger<UpdateSalesOrderCommandHandler> logger,
        UserManager<ApplicationUser> userManager,
        IStockReservationService stockReservationService,
        ISalesOrderService salesOrderService
    ) : IRequestHandler<UpdateSalesOrderCommand, SalesOrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateSalesOrderCommand> _validator = validator;
        private readonly ILogger<UpdateSalesOrderCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IStockReservationService _stockReservationService = stockReservationService;
        private readonly ISalesOrderService _salesOrderService = salesOrderService;

        public async Task<SalesOrderDto> Handle(UpdateSalesOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting update for SalesOrder {OrderId}", request.Id);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for order {OrderId}: {Errors}",
                    request.Id,
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var user = await _userManager.FindByNameAsync(request.ModifiedBy);
            if (user is null)
            {
                _logger.LogWarning("User {ModifiedBy} not found while updating order {OrderId}",
                    request.ModifiedBy, request.Id);

                throw new NotFoundException<ApplicationUser>(request.ModifiedBy);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var customersRepo = _unitOfWork.GetRepository<Customer, Guid>();
                var warehouseRepo = _unitOfWork.GetRepository<WarehouseProduct, int>();
                var ordersRepo = _unitOfWork.GetRepository<SalesOrder, Guid>();

                var order = await ordersRepo.GetAsync(
                   new GetSalesOrderByIdSpecification(request.Id), cancellationToken);
                if (order is null)
                {
                    _logger.LogWarning("SalesOrder {OrderId} not found", request.Id);
                    throw new NotFoundException<SalesOrder>(request.Id);
                }

                if (request.RowVersion is null)
                {
                    _logger.LogWarning("Missing RowVersion for SalesOrder {OrderId}", request.Id);
                    throw new BadRequestException("RowVersion is required.");
                }

                order.AssertRowVersion(request.RowVersion);
                _logger.LogInformation("RowVersion validated for SalesOrder {OrderId}", request.Id);

                if (order.Status != OrderStatus.Pending)
                {
                    _logger.LogWarning(
                        "Attempt to update SalesOrder {OrderId} with status {Status}. Only Pending allowed.",
                        request.Id, order.Status);

                    throw new BadRequestException(
                        $"Only pending orders can be updated. Current status: {order.Status}");
                }

                var customer = await customersRepo.GetAsync(
                   new GetCustomerByIdWithIncludeSpec(request.Dto.CustomerId), cancellationToken)
                   ?? throw new NotFoundException<Customer>(request.Dto.CustomerId);

                var oldIds = order.Items.Select(i => i.ProductId).ToList();
                var newIds = request.Dto.SalesOrderItems.Select(i => i.ProductId).ToList();
                var allIds = oldIds.Union(newIds).ToList();

                var warehouseProducts = await warehouseRepo.GetAllAsync(
                    new GetWarehouseProductsSpecification(allIds), cancellationToken);

                _logger.LogInformation("Releasing old reservations for order {OrderId}", request.Id);
                _stockReservationService.ReleaseOldReservations(order, warehouseProducts);

                _logger.LogInformation("Reserving new stock for order {OrderId}", request.Id);
                await _stockReservationService.ReserveStockAsync(request.Dto.SalesOrderItems, warehouseProducts);

                warehouseRepo.UpdateRange(warehouseProducts);

                order = await _salesOrderService.UpdateSalesOrderAsync(
                   customer,
                   request.Dto,
                   order,
                   warehouseProducts,
                   request.ModifiedBy);

                ordersRepo.Update(order);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("SalesOrder {OrderId} updated successfully", request.Id);

                return _mapper.Map<SalesOrderDto>(order);
            }
            catch (ConflictException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                _logger.LogWarning(ex,
                    "Concurrency conflict while updating SalesOrder {OrderId}",
                    request.Id);

                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                _logger.LogError(ex,
                    "Unexpected error while updating SalesOrder {OrderId}",
                    request.Id);

                throw;
            }
        }
    }
}
