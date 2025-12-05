using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss;
using Khazen.Application.Specification.PurchaseModule.PurchaseOrderSpecifications;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Queries.GetById
{
    internal class GetPurchaseOrderByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetPurchaseOrderByIdHandler> logger)
                : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetPurchaseOrderByIdHandler> _logger = logger;

        public async Task<PurchaseOrderDetailsDto> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting GetPurchaseOrderByIdHandler for PurchaseOrder Id: {PurchaseOrderId}", request.Id);

            try
            {
                var repository = _unitOfWork.GetRepository<PurchaseOrder, Guid>();

                var purchaseOrder = await repository.GetAsync(
                    new GetPurhaseOrderByIdSpec(request.Id),
                    cancellationToken,
                    asNoTracking: true);

                if (purchaseOrder is null)
                {
                    _logger.LogWarning("PurchaseOrder not found. Id: {PurchaseOrderId}", request.Id);
                    throw new NotFoundException<PurchaseOrder>(request.Id);
                }

                _logger.LogInformation("Successfully retrieved PurchaseOrder Id: {PurchaseOrderId}", request.Id);

                return _mapper.Map<PurchaseOrderDetailsDto>(purchaseOrder);
            }
            catch (NotFoundException<PurchaseOrder> ex)
            {
                _logger.LogWarning(ex, "PurchaseOrder not found. Id: {PurchaseOrderId}", request.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving PurchaseOrder Id: {PurchaseOrderId}", request.Id);
                throw new DomainException("Unexpected error occurred while retrieving purchase order.");
            }
        }
    }

}
