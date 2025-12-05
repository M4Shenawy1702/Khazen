using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;
using Khazen.Application.Specification.SalesModule.SalesInvoicesSpecifications;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Queries.GetById
{
    internal class GetSalesInvoiceByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetSalesInvoiceByIdQueryHandler> logger)
        : IRequestHandler<GetSalesInvoiceByIdQuery, SalesInvoiceDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetSalesInvoiceByIdQueryHandler> _logger = logger;

        public async Task<SalesInvoiceDetailsDto> Handle(GetSalesInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching SalesInvoice with ID: {Id}", request.Id);

            var repository = _unitOfWork.GetRepository<SalesInvoice, Guid>();

            var salesInvoice = await repository.GetAsync(
                new GetSalesInvoiceWithIncludesSpecifications(request.Id),
                cancellationToken, true);
            if (salesInvoice is null)
            {
                _logger.LogWarning("SalesInvoice with ID: {Id} not found", request.Id);
                throw new NotFoundException<SalesInvoice>(request.Id);
            }

            _logger.LogInformation("SalesInvoice {Id} retrieved successfully", request.Id);

            return _mapper.Map<SalesInvoiceDetailsDto>(salesInvoice);
        }
    }
}
