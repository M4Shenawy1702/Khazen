using Khazen.Application.Common;
using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Queries.GetAll
{
    public class GetAllPurchaseInvoicesQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<GetAllPurchaseInvoicesQuery> validator,
        ILogger<GetAllPurchaseInvoicesQueryHandler> logger)
                : IRequestHandler<GetAllPurchaseInvoicesQuery, PaginatedResult<PurchaseInvoiceDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetAllPurchaseInvoicesQuery> _validator = validator;
        private readonly ILogger<GetAllPurchaseInvoicesQueryHandler> _logger = logger;

        public async Task<PaginatedResult<PurchaseInvoiceDto>> Handle(
            GetAllPurchaseInvoicesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting GetAllPurchaseInvoices | PageIndex={PageIndex}, PageSize={PageSize}",
                request.QueryParameters.PageIndex,
                request.QueryParameters.PageSize);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

                _logger.LogWarning(
                    "Validation failed for GetAllPurchaseInvoices | Errors={Errors}",
                    string.Join(", ", errorMessages));

                throw new BadRequestException(errorMessages);
            }

            try
            {
                _logger.LogDebug("Fetching PurchaseInvoice list with applied filters");

                var repo = _unitOfWork.GetRepository<PurchaseInvoice, Guid>();

                var invoices = await repo.GetAllAsync(
                   new GetAllPurchaseInvoicesSpec(request.QueryParameters), cancellationToken, asNoTracking: true);

                var data = _mapper.Map<List<PurchaseInvoiceDto>>(invoices);

                var totalCount = await repo.GetCountAsync(new GetAllPurchaseInvoicesCountSpec(request.QueryParameters), cancellationToken);

                _logger.LogInformation(
                    "Completed GetAllPurchaseInvoices | TotalCount={TotalCount}",
                    totalCount);

                return new PaginatedResult<PurchaseInvoiceDto>(
                    request.QueryParameters.PageIndex,
                    request.QueryParameters.PageSize,
                    totalCount,
                    data);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while processing GetAllPurchaseInvoices | PageIndex={Index} | PageSize={Size}",
                    request.QueryParameters.PageIndex,
                    request.QueryParameters.PageSize);

                throw;
            }
        }
    }
}
