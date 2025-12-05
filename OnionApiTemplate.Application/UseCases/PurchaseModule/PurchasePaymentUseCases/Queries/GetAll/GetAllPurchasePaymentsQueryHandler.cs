using Khazen.Application.Common;
using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Application.Specification.PurchaseModule.PurchasePaymentSpecificatiocs;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Queries.GetAll
{
    public class GetAllPurchasePaymentsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<GetAllPurchasePaymentsQuery> validator,
    ILogger<GetAllPurchasePaymentsQueryHandler> logger)
    : IRequestHandler<GetAllPurchasePaymentsQuery, PaginatedResult<PurchasePaymentDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetAllPurchasePaymentsQuery> _validator = validator;
        private readonly ILogger<GetAllPurchasePaymentsQueryHandler> _logger = logger;

        public async Task<PaginatedResult<PurchasePaymentDto>> Handle(GetAllPurchasePaymentsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting GetAllPurchasePaymentsQuery for PageIndex: {PageIndex}, PageSize: {PageSize}",
                request.QueryParameters.PageIndex, request.QueryParameters.PageSize);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for GetAllPurchasePaymentsQuery: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new BadRequestException(validationResult.Errors.Select(i => i.ErrorMessage).ToList());
            }

            var repo = _unitOfWork.GetRepository<PurchasePayment, Guid>();

            var payments = await repo.GetAllAsync(new GetAllPurchasePaymentsSpecifications(request.QueryParameters), cancellationToken);
            var data = _mapper.Map<List<PurchasePaymentDto>>(payments);

            var dataCount = await repo.GetCountAsync(new GetAllPurchasePaymentsCountSpecifications(request.QueryParameters), cancellationToken);

            _logger.LogInformation("Retrieved {Count} purchase payments (TotalCount: {TotalCount}) for PageIndex: {PageIndex}",
                data.Count, dataCount, request.QueryParameters.PageIndex);

            return new PaginatedResult<PurchasePaymentDto>(request.QueryParameters.PageSize, request.QueryParameters.PageIndex, dataCount, data);
        }
    }
}
