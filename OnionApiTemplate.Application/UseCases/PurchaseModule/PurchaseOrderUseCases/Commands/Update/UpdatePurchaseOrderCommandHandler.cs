using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss;
using Khazen.Application.Specification.InventoryModule.ProductSpecifications;
using Khazen.Application.Specification.PurchaseModule.PurchaseOrderSpecifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Update
{
    internal class UpdatePurchaseOrderCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<UpdatePurchaseOrderCommand> validator,
    ILogger<UpdatePurchaseOrderCommandHandler> logger)
        : IRequestHandler<UpdatePurchaseOrderCommand, PurchaseOrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdatePurchaseOrderCommand> _validator = validator;
        private readonly ILogger<UpdatePurchaseOrderCommandHandler> _logger = logger;

        public async Task<PurchaseOrderDto> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting UpdatePurchaseOrderCommandHandler for PurchaseOrder Id: {PurchaseOrderId}", request.Id);
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for PurchaseOrder Id {PurchaseOrderId}: {Errors}",
                        request.Id,
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var repository = _unitOfWork.GetRepository<PurchaseOrder, Guid>();
                var purchaseOrder = await repository.GetAsync(new GetPurhaseOrderByIdSpec(request.Id), cancellationToken)
                    ?? throw new NotFoundException<PurchaseOrder>(request.Id);

                purchaseOrder.SetRowVersion(request.Dto.RowVersion);

                if (purchaseOrder.OrderNumber != request.Dto.OrderNumber)
                {
                    var duplicateExists = await repository.AnyAsync(po => po.OrderNumber == request.Dto.OrderNumber && po.Id != request.Id, cancellationToken);
                    if (duplicateExists)
                    {
                        _logger.LogWarning("OrderNumber {OrderNumber} already exists for another order.", request.Dto.OrderNumber);
                        throw new BadRequestException("OrderNumber already exists.");
                    }
                }

                purchaseOrder.Items.Clear();
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

                purchaseOrder.Modify(request.Dto.SupplierId, request.Dto.OrderNumber, request.Dto.DeliveryDate, request.ModifiedBy, request.Dto.Notes);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("UpdatePurchaseOrderCommandHandler completed for PurchaseOrder Id: {PurchaseOrderId}", request.Id);

                return _mapper.Map<PurchaseOrderDto>(purchaseOrder);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException("The purchase order was updated by another user. Please reload and try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdatePurchaseOrderCommandHandler for PurchaseOrder Id: {PurchaseOrderId}", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
