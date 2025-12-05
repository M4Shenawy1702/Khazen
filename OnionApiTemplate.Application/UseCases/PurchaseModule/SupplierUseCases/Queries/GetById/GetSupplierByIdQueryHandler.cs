using Khazen.Application.DOTs.PurchaseModule.SupplierDtos;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Queries.GetById
{
    using Microsoft.Extensions.Logging;

    namespace Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Queries.GetById
    {
        internal class GetSupplierByIdQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetSupplierByIdQueryHandler> logger)
            : IRequestHandler<GetSupplierByIdQuery, SupplierDto>
        {
            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            private readonly IMapper _mapper = mapper;
            private readonly ILogger<GetSupplierByIdQueryHandler> _logger = logger;

            public async Task<SupplierDto> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    _logger.LogInformation("Fetching supplier with ID: {SupplierId}", request.Id);

                    var repo = _unitOfWork.GetRepository<Supplier, Guid>();
                    var supplier = await repo.GetByIdAsync(request.Id, cancellationToken);
                    if (supplier == null)
                    {
                        _logger.LogInformation("Supplier with ID: {SupplierId} not found.", request.Id);
                        throw new NotFoundException<Supplier>(request.Id);
                    }

                    _logger.LogInformation("Supplier with ID: {SupplierId} fetched successfully.", request.Id);

                    return _mapper.Map<SupplierDto>(supplier);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching supplier with ID: {SupplierId}", request.Id);
                    throw;
                }
            }
        }
    }
}
