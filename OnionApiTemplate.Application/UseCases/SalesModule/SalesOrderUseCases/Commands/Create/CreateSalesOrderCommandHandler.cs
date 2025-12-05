using Khazen.Application.Common.Interfaces.SalesModule.ISalesOrderModule;
using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;
using Khazen.Application.Specification.InventoryModule.WareHouseProductSpecifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Create
{
    internal class CreateSalesOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateSalesOrderCommand> validator,
        ILogger<CreateSalesOrderCommandHandler> logger,
        ISalesOrderService salesOrederService,
        IStockReservationService stockReservationService,
        UserManager<ApplicationUser> userManager
    ) : IRequestHandler<CreateSalesOrderCommand, SalesOrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateSalesOrderCommand> _validator = validator;
        private readonly ILogger<CreateSalesOrderCommandHandler> _logger = logger;
        private readonly ISalesOrderService _salesOrederService = salesOrederService;
        private readonly IStockReservationService _stockReservationService = stockReservationService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<SalesOrderDto> Handle(CreateSalesOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "CreateSalesOrder started for CustomerId: {CustomerId}",
                request.Dto.CustomerId
            );

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed: {@Errors}", validationResult.Errors);
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var user = await _userManager.FindByNameAsync(request.CreatedBy);
            if (user is null)
            {
                _logger.LogInformation("User not found. UserName: {CreatedBy}", request.CreatedBy);
                throw new NotFoundException<ApplicationUser>(request.CreatedBy);
            }

            if (!request.Dto.SalesOrderItems.Any())
                throw new BadRequestException("Sales order must contain at least one item.");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
                var customer = await customerRepo.GetByIdAsync(request.Dto.CustomerId, cancellationToken);
                if (customer is null)
                {
                    _logger.LogWarning("Customer not found. CustomerId: {CustomerId}", request.Dto.CustomerId);
                    throw new NotFoundException<Customer>(request.Dto.CustomerId);
                }

                var productIds = request.Dto.SalesOrderItems.Select(i => i.ProductId).ToList();
                var warehouseProductsRepo = _unitOfWork.GetRepository<WarehouseProduct, int>();

                var warehouseProducts = await warehouseProductsRepo.GetAllAsync(
                    new GetWarehouseProductsSpecification(productIds),
                    cancellationToken
                );

                await _stockReservationService.ReserveStockAsync(request.Dto.SalesOrderItems, warehouseProducts);

                foreach (var wp in warehouseProducts)
                    warehouseProductsRepo.Update(wp);

                var order = _salesOrederService.CreateSalesOreder(
                   customer,
                   request.Dto,
                   warehouseProducts,
                   request.CreatedBy
               );

                var orderRepo = _unitOfWork.GetRepository<SalesOrder, Guid>();
                await orderRepo.AddAsync(order, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("SalesOrder created successfully. OrderId: {OrderId}", order.Id);

                return _mapper.Map<SalesOrderDto>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating SalesOrder. Rolling back...");
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
