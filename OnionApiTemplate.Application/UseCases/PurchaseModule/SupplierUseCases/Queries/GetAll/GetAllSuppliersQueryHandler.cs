using Khazen.Application.DOTs.PurchaseModule.SupplierDtos;
using Khazen.Application.Specification.PurchaseModule.SupplierSpecifications;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Queries.GetAll
{
    using Microsoft.Extensions.Logging;

    namespace Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Queries.GetAll
    {
        internal class GetAllSuppliersQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<GetAllSuppliersQueryHandler> logger)
            : IRequestHandler<GetAllSuppliersQuery, List<SupplierDto>>
        {
            private readonly IUnitOfWork _unitOfWork = unitOfWork;
            private readonly IMapper _mapper = mapper;
            private readonly ILogger<GetAllSuppliersQueryHandler> _logger = logger;

            public async Task<List<SupplierDto>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    _logger.LogInformation("Fetching all suppliers.");

                    var repo = _unitOfWork.GetRepository<Supplier, Guid>();
                    var suppliers = await repo.GetAllAsync(new GetAllSuppliersSpecification(), cancellationToken);

                    _logger.LogInformation("Fetched {Count} suppliers successfully.", suppliers.ToList().Count);

                    return _mapper.Map<List<SupplierDto>>(suppliers);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching suppliers.");
                    throw;
                }
            }
        }
    }

}
