using Khazen.Application.Common;
using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;
using Khazen.Application.Specification.PurchaseModule.PurchaseReceiptSpecifications;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Queries.GetAll
{
    public class GetAllPurchaseReceiptsHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<GetAllPurchaseReceiptsQuery> validator,
    ILogger<GetAllPurchaseReceiptsHandler> logger
) : IRequestHandler<GetAllPurchaseReceiptsQuery, PaginatedResult<PurchaseReceiptDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetAllPurchaseReceiptsQuery> _validator = validator;
        private readonly ILogger<GetAllPurchaseReceiptsHandler> _logger = logger;

        public async Task<PaginatedResult<PurchaseReceiptDto>> Handle(GetAllPurchaseReceiptsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllPurchaseReceiptsQuery: PageIndex={PageIndex}, PageSize={PageSize}",
                request.QueryParameters.PageIndex, request.QueryParameters.PageSize);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for GetAllPurchaseReceiptsQuery: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var repo = _unitOfWork.GetRepository<PurchaseReceipt, Guid>();

            _logger.LogInformation("Fetching purchase receipts from database...");
            var receipts = await repo.GetAllAsync(
                new GetAllPurchaseReceiptsByQueryParametersSpec(request.QueryParameters),
                cancellationToken,
                true
            );

            var count = await repo.GetCountAsync(
                new GetAllPurchaseReceiptsByQueryParametersSpec(request.QueryParameters),
                cancellationToken
            );

            _logger.LogInformation("Fetched {Count} purchase receipts", receipts.Count());

            var data = _mapper.Map<IEnumerable<PurchaseReceiptDto>>(receipts);

            _logger.LogInformation("Mapped purchase receipts to DTOs successfully");

            return new PaginatedResult<PurchaseReceiptDto>(
                request.QueryParameters.PageIndex,
                request.QueryParameters.PageSize,
                count,
                data
            );
        }
    }
}
