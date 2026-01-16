using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss;
using Khazen.Application.Specification.InventoryModule.ProductSpecifications;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Create
{
    internal class CreatePurchaseOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreatePurchaseOrderCommand> validator,
        ILogger<CreatePurchaseOrderCommandHandler> logger
        , UserManager<ApplicationUser> userManager)
                : IRequestHandler<CreatePurchaseOrderCommand, PurchaseOrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreatePurchaseOrderCommand> _validator = validator;
        private readonly ILogger<CreatePurchaseOrderCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<PurchaseOrderDto> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreatePO: Starting for Order {OrderNo} with {ItemCount} lines",
                request.Dto.OrderNumber, request.Dto.Items.Count);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("CreatePO: Validation failed for {OrderNo}. Errors: {Errors}",
                    request.Dto.OrderNumber, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var supplierTask = _unitOfWork.GetRepository<Supplier, Guid>().GetByIdAsync(request.Dto.SupplierId, cancellationToken);
            var userTask = _userManager.FindByNameAsync(request.CurrentUserId);
            var duplicateCheckTask = _unitOfWork.GetRepository<PurchaseOrder, Guid>()
                .AnyAsync(po => po.OrderNumber == request.Dto.OrderNumber && po.Status != PurchaseOrderStatus.Cancelled, cancellationToken);

            await Task.WhenAll(supplierTask, userTask, duplicateCheckTask);

            var supplier = await supplierTask;

            if (supplier is null)
            {
                _logger.LogError("CreatePO: Supplier {SupplierId} not found during processing of {OrderNo}", request.Dto.SupplierId, request.Dto.OrderNumber);
                throw new NotFoundException<Supplier>(request.Dto.SupplierId);
            }
            if (supplier.IsDeleted == true)
            {
                _logger.LogError("CreatePO: Supplier {SupplierId} is deleted during processing of {OrderNo}", request.Dto.SupplierId, request.Dto.OrderNumber);
                throw new BadRequestException("Supplier is not active.");
            }
            var user = await userTask ?? throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            if (await duplicateCheckTask)
            {
                _logger.LogWarning("CreatePO: Duplicate OrderNumber {OrderNo} detected", request.Dto.OrderNumber);
                throw new BadRequestException("OrderNumber already exists.");
            }

            var purchaseOrder = new PurchaseOrder(request.Dto.SupplierId, request.Dto.OrderNumber, request.Dto.DeliveryDate, user.Id, request.Dto.Notes);

            var productIds = request.Dto.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _unitOfWork.GetRepository<Product, Guid>()
                .GetAllAsync(new GetAllProductsByIdsSpecification(productIds), cancellationToken, asNoTracking: true);

            var productsDict = products.ToDictionary(p => p.Id, p => p);

            foreach (var itemDto in request.Dto.Items)
            {
                if (!productsDict.TryGetValue(itemDto.ProductId, out var product))
                {
                    _logger.LogError("CreatePO: Product {ProductId} not found during processing of {OrderNo}", itemDto.ProductId, request.Dto.OrderNumber);
                    throw new NotFoundException<Product>(itemDto.ProductId);
                }

                var domainItem = new PurchaseOrderItem(product.Id, itemDto.Quantity, itemDto.ExpectedUnitPrice);
                purchaseOrder.AddItem(domainItem);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                _logger.LogDebug("CreatePO: Saving Order {OrderNo} to DB", purchaseOrder.OrderNumber);

                await _unitOfWork.GetRepository<PurchaseOrder, Guid>().AddAsync(purchaseOrder, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("CreatePO: SUCCESS. Order {OrderNo} created with ID {Id}",
                    purchaseOrder.OrderNumber, purchaseOrder.Id);

                return _mapper.Map<PurchaseOrderDto>(purchaseOrder);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "CreatePO: Failed for {OrderNo}. Rolling back transaction", request.Dto.OrderNumber);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
