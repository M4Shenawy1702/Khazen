using Khazen.Application.Common;
using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;
using Khazen.Application.Specification.SalesModule.SalesInvoicesSpecifications;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Queries.GetAll
{
    internal class GetAllSalesInvoicesQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<GetAllSalesInvoicesQuery> validator,
        ILogger<GetAllSalesInvoicesQueryHandler> logger)
        : IRequestHandler<GetAllSalesInvoicesQuery, PaginatedResult<SalesInvoiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetAllSalesInvoicesQuery> _validator = validator;
        private readonly ILogger<GetAllSalesInvoicesQueryHandler> _logger = logger;

        public async Task<PaginatedResult<SalesInvoiceDto>> Handle(GetAllSalesInvoicesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Handling GetAllSalesInvoicesQuery: PageIndex={PageIndex}, PageSize={PageSize}, Filters={@Filters}",
                request.QueryParameters.PageIndex,
                request.QueryParameters.PageSize,
                request.QueryParameters);

            // Validate
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for GetAllSalesInvoicesQuery: {@Errors}", validationResult.Errors);
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var repo = _unitOfWork.GetRepository<SalesInvoice, Guid>();

            _logger.LogInformation("Fetching SalesInvoices from database...");
            var salesInvoices = await repo.GetAllAsync(new GetAllSalesInvoicesSpecifications(request.QueryParameters), cancellationToken, true);

            _logger.LogInformation("Mapping SalesInvoices to DTOs...");
            var salesInvoicesDto = _mapper.Map<List<SalesInvoiceDto>>(salesInvoices);

            _logger.LogInformation("Fetching total count for pagination...");
            var totalCount = await repo.GetCountAsync(new GetAllSalesInvoicesCountSpecifications(request.QueryParameters), cancellationToken, true);

            _logger.LogInformation("Successfully fetched {Count} sales invoices.", totalCount);

            return new PaginatedResult<SalesInvoiceDto>(
                request.QueryParameters.PageIndex,
                request.QueryParameters.PageSize,
                totalCount,
                salesInvoicesDto);
        }
    }
}
