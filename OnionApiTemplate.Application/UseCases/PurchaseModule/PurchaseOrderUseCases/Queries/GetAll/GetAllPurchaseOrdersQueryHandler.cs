using Khazen.Application.Common;
using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss;
using Khazen.Application.Specification.PurchaseModule.PurchaseOrderSpecifications;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Queries.GetAll
{
    internal class GetAllPurchaseOrdersHandler
        : IRequestHandler<GetAllPurchaseOrdersQuery, PaginatedResult<PurchaseOrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<GetAllPurchaseOrdersQuery> _validator;
        private readonly ILogger<GetAllPurchaseOrdersHandler> _logger;

        public GetAllPurchaseOrdersHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<GetAllPurchaseOrdersQuery> validator,
            ILogger<GetAllPurchaseOrdersHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
            _logger = logger;
        }

        public async Task<PaginatedResult<PurchaseOrderDto>> Handle(GetAllPurchaseOrdersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting GetAllPurchaseOrdersHandler with parameters: {@QueryParams}", request.QueryParameters);

            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var repository = _unitOfWork.GetRepository<PurchaseOrder, Guid>();

                var purchaseOrders = await repository.GetAllAsync(
                    new GetAllPurchaseOrdersSpec(request.QueryParameters),
                    cancellationToken,
                    asNoTracking: true);

                var data = _mapper.Map<IEnumerable<PurchaseOrderDto>>(purchaseOrders);

                var count = await repository.GetCountAsync(
                    new GetAllPurchaseOrdersCountSpec(request.QueryParameters),
                    cancellationToken);

                _logger.LogInformation("Completed GetAllPurchaseOrdersHandler. Returned {Count} items.", data.Count());

                return new PaginatedResult<PurchaseOrderDto>(
                    request.QueryParameters.PageIndex,
                    request.QueryParameters.PageSize,
                    count,
                    data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllPurchaseOrdersHandler. Parameters: {@QueryParams}", request.QueryParameters);
                throw;
            }
        }
    }
}
