using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;
using Khazen.Application.Specification.PurchaseModule.PurchaseReceiptSpecifications;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Queries.GetById
{
    public class GetPurchaseReceiptByIdHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetPurchaseReceiptByIdHandler> logger
    ) : IRequestHandler<GetPurchaseReceiptByIdQuery, PurchaseReceiptDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetPurchaseReceiptByIdHandler> _logger = logger;

        public async Task<PurchaseReceiptDetailsDto> Handle(GetPurchaseReceiptByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching Purchase Receipt by ID: {PurchaseReceiptId}", request.Id);

            var repo = _unitOfWork.GetRepository<PurchaseReceipt, Guid>();

            var receipt = await repo.GetAsync(
                new GetPurchaseReceiptWithItemsByIdSpec(request.Id),
                cancellationToken,
                true
            );

            if (receipt is null)
            {
                _logger.LogWarning("Purchase Receipt with ID {PurchaseReceiptId} not found", request.Id);
                throw new NotFoundException<PurchaseReceipt>(request.Id);
            }

            _logger.LogInformation("Purchase Receipt with ID {PurchaseReceiptId} retrieved successfully", request.Id);

            return _mapper.Map<PurchaseReceiptDetailsDto>(receipt);
        }
    }
}
