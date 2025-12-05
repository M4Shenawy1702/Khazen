using Khazen.Application.Common.Interfaces.SalesModule.ISalesOrderModule;
using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;
using Khazen.Application.Specification.InventoryModule.ProductSpecifications;
using Khazen.Application.Specification.SalesModule.SalesOrdersSpecifications;
using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Confirm;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

internal class ConfirmOrderCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<ConfirmOrderCommand> validator,
    UserManager<ApplicationUser> userManager,
    ILogger<ConfirmOrderCommandHandler> logger,
    IStockReservationService stockReservationService)
    : IRequestHandler<ConfirmOrderCommand, SalesOrderDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<ConfirmOrderCommand> _validator = validator;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILogger<ConfirmOrderCommandHandler> _logger = logger;
    private readonly IStockReservationService _stockReservationService = stockReservationService;

    public async Task<SalesOrderDto> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting order confirmation. OrderId: {OrderId}, ConfirmedBy: {User}", request.Id, request.ConfirmedBy);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for order confirmation. Errors: {@Errors}", validationResult.Errors);
            throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        }

        var user = await _userManager.FindByNameAsync(request.ConfirmedBy);
        if (user is null)
        {
            _logger.LogWarning("User not found while confirming order. UserName: {ConfirmedBy}", request.ConfirmedBy);
            throw new NotFoundException<ApplicationUser>(request.ConfirmedBy);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var salesOrdersRepository = _unitOfWork.GetRepository<SalesOrder, Guid>();
            var salesOrder = await salesOrdersRepository.GetAsync(new GetSalesOrderByIdSpecification(request.Id), cancellationToken);

            if (salesOrder is null)
            {
                _logger.LogWarning("SalesOrder not found while confirming order. OrderId: {OrderId}", request.Id);
                throw new NotFoundException<SalesOrder>(request.Id);
            }

            _logger.LogDebug("Fetched SalesOrder with ID {OrderId}. Current Status: {Status}", salesOrder.Id, salesOrder.Status);

            if (request.RowVersion is null)
            {
                _logger.LogWarning("RowVersion missing for order {OrderId}", request.Id);
                throw new BadRequestException("RowVersion is required.");
            }

            salesOrder.AssertRowVersion(request.RowVersion);
            _logger.LogInformation("RowVersion check passed for order {OrderId}", request.Id);

            if (salesOrder.Status != OrderStatus.Pending)
            {
                _logger.LogWarning("Order {OrderId} cannot be confirmed. Current status: {Status}", salesOrder.Id, salesOrder.Status);
                throw new BadRequestException($"Only pending orders can be confirmed. Current status: {salesOrder.Status}");
            }

            var productIds = salesOrder.Items.Select(i => i.ProductId).ToList();

            var productsRepository = _unitOfWork.GetRepository<Product, Guid>();
            var products = await productsRepository.GetAllAsync(new GetAllProductsByIdsSpecification(productIds), cancellationToken);

            var productLookup = products.ToDictionary(p => p.Id);

            await _stockReservationService.ValidateReservedQuantitiesAsync(salesOrder, productLookup);

            var oldStatus = salesOrder.Status;
            salesOrder.MarkAsConfirmed();
            _logger.LogInformation(
                "SalesOrder {OrderId} confirmed by {User}. Status changed from {OldStatus} to {NewStatus}",
                salesOrder.Id, user.UserName, oldStatus, salesOrder.Status
            );

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

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
            _logger.LogError(ex, "Error while confirming SalesOrder {OrderId}. Rolling back transaction.", request.Id);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}