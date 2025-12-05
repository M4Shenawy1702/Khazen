using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;
using Khazen.Application.Specification.SalesModule.OrderPaymentSpecification;
using Khazen.Application.UseCases.SalesModule.SalesOrderPaymentUseCases.Queries.GetById;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Queries.GetById
{
    internal class GetSalesInvoicePaymentByIdHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<GetSalesInvoicePaymentByIdHandler> logger
    ) : IRequestHandler<GetSalesInvoicePaymentByIdQuery, SalesInvoiceDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetSalesInvoicePaymentByIdHandler> _logger = logger;

        public async Task<SalesInvoiceDetailsDto> Handle(GetSalesInvoicePaymentByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching SalesInvoicePayment by Id: {PaymentId}", request.Id);

            var repo = _unitOfWork.GetRepository<SalesInvoicePayment, Guid>();

            var payment = await repo.GetAsync(
                new GetSalesOrderPaymentByIdSpecification(request.Id),
                cancellationToken,
                 true
            );

            if (payment == null)
            {
                _logger.LogWarning("SalesInvoicePayment {PaymentId} not found", request.Id);
                throw new NotFoundException<SalesInvoicePayment>(request.Id);
            }

            _logger.LogInformation("SalesInvoicePayment {PaymentId} retrieved successfully. Mapping to DTO.", request.Id);

            return _mapper.Map<SalesInvoiceDetailsDto>(payment);
        }
    }
}
