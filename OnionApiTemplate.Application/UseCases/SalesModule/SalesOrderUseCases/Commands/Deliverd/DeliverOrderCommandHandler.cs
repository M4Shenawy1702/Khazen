using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;
using Khazen.Application.Specification.SalesModule.SalesOrdersSpecifications;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Deliverd
{
    internal class DeliverOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<DeliverOrderCommand> validator,
        ILogger<DeliverOrderCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<DeliverOrderCommand, SalesOrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<DeliverOrderCommand> _validator = validator;
        private readonly ILogger<DeliverOrderCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<SalesOrderDto> Handle(DeliverOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start delivery process for order {OrderId} by user {User}", request.Id, request.DeliveredBy);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for order {OrderId}: {Errors}", request.Id, validationResult.Errors.Select(e => e.ErrorMessage));
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var user = await _userManager.FindByNameAsync(request.DeliveredBy);
            if (user is null)
            {
                _logger.LogWarning("User not found while delivering order. UserName: {DeliveredBy}", request.DeliveredBy);
                throw new NotFoundException<ApplicationUser>(request.DeliveredBy);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var salesOrdersRepository = _unitOfWork.GetRepository<SalesOrder, Guid>();
                var salesOrder = await salesOrdersRepository.GetAsync(new GetSalesOrderByIdSpecification(request.Id), cancellationToken);

                if (salesOrder is null)
                {
                    _logger.LogWarning("Order {OrderId} not found", request.Id);
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

                if (salesOrder.Status != OrderStatus.Shipped)
                {
                    _logger.LogWarning("Order {OrderId} status {Status} invalid for delivery", request.Id, salesOrder.Status);
                    throw new BadRequestException($"Only shipped orders can be delivered. Current status: {salesOrder.Status}");
                }

                salesOrder.MarkAsDelivered(request.DeliveredBy);
                _logger.LogInformation("Order {OrderId} marked as delivered by {User}", salesOrder.Id, request.DeliveredBy);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Delivery transaction committed successfully for order {OrderId}", request.Id);

                return _mapper.Map<SalesOrderDto>(salesOrder);
            }
            catch (ConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict while delivering order {OrderId}", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while delivering order {OrderId}", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
