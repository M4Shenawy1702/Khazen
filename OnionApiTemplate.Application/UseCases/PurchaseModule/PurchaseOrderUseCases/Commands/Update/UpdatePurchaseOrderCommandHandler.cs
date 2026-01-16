using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss;
using Khazen.Application.Specification.InventoryModule.ProductSpecifications;
using Khazen.Application.Specification.PurchaseModule.PurchaseOrderSpecifications;
using Khazen.Domain.Common.Enums;
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
    ILogger<UpdatePurchaseOrderCommandHandler> logger,
    UserManager<ApplicationUser> userManager)
        : IRequestHandler<UpdatePurchaseOrderCommand, PurchaseOrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdatePurchaseOrderCommand> _validator = validator;
        private readonly ILogger<UpdatePurchaseOrderCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<PurchaseOrderDto> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdatePO: Starting for ID {Id}", request.Id);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

            var repo = _unitOfWork.GetRepository<PurchaseOrder, Guid>();
            var poTask = repo.GetAsync(new GetPurhaseOrderByIdSpec(request.Id), cancellationToken);
            var supplierTask = _unitOfWork.GetRepository<Supplier, Guid>().GetByIdAsync(request.Dto.SupplierId, cancellationToken);
            var userTask = _userManager.FindByIdAsync(request.CurrentUserId);

            await Task.WhenAll(userTask, supplierTask, poTask);

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
            var user = await userTask;
            if (user is null)
            {
                _logger.LogError("CreatePO: User {UserId} not found during processing of {OrderNo}", request.CurrentUserId, request.Dto.OrderNumber);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }
            var purchaseOrder = await poTask;
            if (purchaseOrder is null)
            {
                _logger.LogError("CreatePO: PO {Id} not found during processing of {OrderNo}", request.Id, request.Dto.OrderNumber);
                throw new NotFoundException<PurchaseOrder>(request.Id);
            }

            if (purchaseOrder.Status != PurchaseOrderStatus.Pending)
            {
                _logger.LogWarning("UpdatePO: Attempted to edit Order {No} in status {Status}", purchaseOrder.OrderNumber, purchaseOrder.Status);
                throw new BadRequestException("Only Draft or Open orders can be modified.");
            }

            if (purchaseOrder.OrderNumber != request.Dto.OrderNumber)
            {
                var duplicate = await repo.AnyAsync(po => po.OrderNumber == request.Dto.OrderNumber && po.Id != request.Id, cancellationToken);
                if (duplicate) throw new BadRequestException("OrderNumber already exists.");
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                purchaseOrder.SetRowVersion(request.Dto.RowVersion);

                purchaseOrder.Items.Clear();

                var productIds = request.Dto.Items.Select(i => i.ProductId).Distinct().ToList();
                var products = await _unitOfWork.GetRepository<Product, Guid>()
                    .GetAllAsync(new GetAllProductsByIdsSpecification(productIds), cancellationToken, true);
                var productsDict = products.ToDictionary(p => p.Id, p => p);

                foreach (var itemDto in request.Dto.Items)
                {
                    if (!productsDict.TryGetValue(itemDto.ProductId, out var product))
                        throw new NotFoundException<Product>(itemDto.ProductId);

                    purchaseOrder.AddItem(new PurchaseOrderItem(product.Id, itemDto.Quantity, itemDto.ExpectedUnitPrice));
                }

                purchaseOrder.Modify(request.Dto.SupplierId, request.Dto.OrderNumber, request.Dto.DeliveryDate, user.Id, request.Dto.Notes);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("UpdatePO: SUCCESS. Order {No} updated by {User}", purchaseOrder.OrderNumber, user.UserName);
                return _mapper.Map<PurchaseOrderDto>(purchaseOrder);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning("UpdatePO: Concurrency conflict for {Id}", request.Id);
                throw new ConcurrencyException("The order was updated by someone else.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdatePO: Critical failure for {Id}", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
