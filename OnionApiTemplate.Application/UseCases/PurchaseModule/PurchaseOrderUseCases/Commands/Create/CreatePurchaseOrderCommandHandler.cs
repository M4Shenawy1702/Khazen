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
            _logger.LogInformation("Strarting CreatePurchaseOrderCommandHandler for purchase Order Number: {PurchaseOrderNumber}", request.Dto.OrderNumber);
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for : {PurchaseOrderNumber}: {Errors}",
                      request.Dto.OrderNumber,
                      string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var supplier = await _unitOfWork.GetRepository<Supplier, Guid>().GetByIdAsync(request.Dto.SupplierId, cancellationToken);
                if (supplier is null)
                {
                    _logger.LogWarning("Supplier with id {SupplierId} was not found ", request.Dto.SupplierId);
                    throw new NotFoundException<Supplier>(request.Dto.SupplierId);
                }


                var user = await _userManager.FindByNameAsync(request.CreatedBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserId: {CreatedBy}", request.CreatedBy);
                    throw new NotFoundException<ApplicationUser>(request.CreatedBy);
                }

                var existingOrder = await _unitOfWork.GetRepository<PurchaseOrder, Guid>()
                   .AnyAsync(po => po.OrderNumber == request.Dto.OrderNumber && po.Status != PurchaseOrderStatus.Cancelled, cancellationToken);
                if (existingOrder)
                {
                    _logger.LogWarning("OrderNumber {OrderNumber} already exists", request.Dto.OrderNumber);
                    throw new BadRequestException("OrderNumber already exists.");
                }

                var purchaseOrder = new PurchaseOrder(request.Dto.SupplierId, request.Dto.OrderNumber, request.Dto.DeliveryDate, request.CreatedBy, request.Dto.Notes);

                var productIds = request.Dto.Items.Select(i => i.ProductId).Distinct().ToList();
                var products = await _unitOfWork.GetRepository<Product, Guid>()
                    .GetAllAsync(new GetAllProductsByIdsSpecification(productIds), cancellationToken, asNoTracking: true);
                var productsDict = products.ToDictionary(p => p.Id, p => p);

                foreach (var itemDto in request.Dto.Items)
                {

                    if (!productsDict.TryGetValue(itemDto.ProductId, out var product))
                    {
                        _logger.LogWarning("Product with id {ProductId} was not found ", itemDto.ProductId);
                        throw new NotFoundException<Product>(itemDto.ProductId);
                    }

                    var domainItem = new PurchaseOrderItem(product.Id, itemDto.Quantity, itemDto.ExpectedUnitPrice);
                    purchaseOrder.AddItem(domainItem);
                }

                var purchaseOrderRepository = _unitOfWork.GetRepository<PurchaseOrder, Guid>();
                await purchaseOrderRepository.AddAsync(purchaseOrder, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("CreatePurchaseOrderCommandHandler completed for purchase Order Number: {PurchaseOrderNumber}", request.Dto.OrderNumber);

                return _mapper.Map<PurchaseOrderDto>(purchaseOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreatePurchaseOrderCommandHandler for purchaseOrder Number{PurchaseOrderNumber}", request.Dto.OrderNumber);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
