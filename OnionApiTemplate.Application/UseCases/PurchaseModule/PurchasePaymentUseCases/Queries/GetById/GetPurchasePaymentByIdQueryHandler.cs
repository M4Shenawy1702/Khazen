using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Application.Specification.PurchaseModule.PurchasePaymentSpecificatiocs;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Queries.GetById
{
    public class GetPurchasePaymentByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<GetPurchasePaymentByIdQueryHandler> logger)
    : IRequestHandler<GetPurchasePaymentByIdQuery, PurchasePaymentDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetPurchasePaymentByIdQueryHandler> _logger = logger;

        public async Task<PurchasePaymentDetailsDto> Handle(GetPurchasePaymentByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                _logger.LogWarning("GetPurchasePaymentByIdQuery called with empty Id.");
                throw new BadRequestException("Payment Id cannot be empty.");
            }

            _logger.LogInformation("Fetching purchase payment with Id: {PaymentId}", request.Id);

            var repo = _unitOfWork.GetRepository<PurchasePayment, Guid>();
            var payment = await repo.GetAsync(new GetPurcasePaymentByIdWithIncludesSpecifications(request.Id), cancellationToken);

            if (payment is null)
            {
                _logger.LogWarning("Purchase payment not found. Id: {PaymentId}", request.Id);
                throw new NotFoundException<PurchasePayment>(request.Id);
            }

            _logger.LogInformation("Purchase payment retrieved successfully. Id: {PaymentId}", request.Id);
            return _mapper.Map<PurchasePaymentDetailsDto>(payment);
        }
    }

}
