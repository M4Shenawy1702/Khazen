using Khazen.Application.DOTs.InventoryModule.WarehouseDtos;
using Khazen.Domain.Entities.InventoryModule;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Queries.GetAll
{
    internal class GetAllWarehousesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllWarehousesQueryHandler> logger)
        : IRequestHandler<GetAllWarehousesQuery, IEnumerable<WarehouseDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllWarehousesQueryHandler> _logger = logger;

        public async Task<IEnumerable<WarehouseDto>> Handle(GetAllWarehousesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Start GetAllWarehousesQueryHandler");
                var repo = _unitOfWork.GetRepository<Warehouse, Guid>();
                var warehouses = await repo.GetAllAsync(cancellationToken, false);
                _logger.LogInformation("Retrieved {Count} warehouses", warehouses.Count());
                return _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllWarehousesQueryHandler");
                throw;
            }
        }
    }
}
