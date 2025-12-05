using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Queries.GetById
{
    public class GetPurchaseInvoiceByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetPurchaseInvoiceByIdQueryHandler> logger)
                : IRequestHandler<GetPurchaseInvoiceByIdQuery, PurchaseInvoiceDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetPurchaseInvoiceByIdQueryHandler> _logger = logger;

        public async Task<PurchaseInvoiceDetailsDto> Handle(
            GetPurchaseInvoiceByIdQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Fetching PurchaseInvoice by Id={InvoiceId}",
                request.Id);

            try
            {
                var repo = _unitOfWork.GetRepository<PurchaseInvoice, Guid>();

                _logger.LogDebug(
                    "Executing specification GetPurchaseInvoiceWithAllIncludesByIdSpec for InvoiceId={InvoiceId}",
                    request.Id);

                var invoice = await repo.GetAsync(
                    new GetPurchaseInvoiceWithAllIncludesByIdSpec(request.Id),
                    cancellationToken,
                    asNoTracking: true);

                if (invoice == null)
                {
                    _logger.LogWarning(
                        "PurchaseInvoice not found for InvoiceId={InvoiceId}",
                        request.Id);

                    throw new NotFoundException<PurchaseInvoice>(request.Id);
                }

                _logger.LogInformation(
                    "Successfully retrieved PurchaseInvoice InvoiceId={InvoiceId}",
                    request.Id);

                return _mapper.Map<PurchaseInvoiceDetailsDto>(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while fetching PurchaseInvoice InvoiceId={InvoiceId}",
                    request.Id);

                throw;
            }
        }
    }
}
