using Khazen.Application.Common;
using Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos;
using Khazen.Application.Specification.SalesModule.OrderPaymentSpecification;
using Khazen.Application.UseCases.SalesModule.SalesOrderPaymentUseCases.Queries.GetAll;
using Khazen.Domain.Entities.SalesModule;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Queries.GetAll
{
    internal class GetAllSalesInvoicePaymentsHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<GetAllSalesInvoicePaymentsHandler> logger
    ) : IRequestHandler<GetAllSalesInvoicePaymentsQuery, PaginatedResult<SalesInvoicePaymentDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllSalesInvoicePaymentsHandler> _logger = logger;

        public async Task<PaginatedResult<SalesInvoicePaymentDto>> Handle(GetAllSalesInvoicePaymentsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Fetching SalesInvoicePayments | PageIndex: {PageIndex}, PageSize: {PageSize}, Filters: {@Filters}",
                request.QueryParameters.PageIndex,
                request.QueryParameters.PageSize,
                request.QueryParameters
            );

            var repo = _unitOfWork.GetRepository<SalesInvoicePayment, Guid>();

            var payments = await repo.GetAllAsync(new GetAllSalesOrderPaymentsSpecification(request.QueryParameters), cancellationToken, true);
            var count = await repo.GetCountAsync(new GetAllSalesOrderPaymentsCountSpecification(request.QueryParameters), cancellationToken, true);

            _logger.LogInformation(
                "Retrieved {Count} SalesInvoicePayments (PagedCount: {PagedCount})",
                count,
                payments.ToList().Count
            );

            var data = _mapper.Map<List<SalesInvoicePaymentDto>>(payments);

            _logger.LogInformation("Mapped SalesInvoicePayments to DTO list. Returning paginated result.");

            return new PaginatedResult<SalesInvoicePaymentDto>(
                request.QueryParameters.PageIndex,
                request.QueryParameters.PageSize,
                count,
                data
            );
        }
    }
}
