using Khazen.Application.DOTs.InventoryModule.WarehouseDtos;
using Khazen.Application.Specification.InventoryModule.WareHouseSpesifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Queries.GetById
{
    internal class GetWarehouseByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetWarehouseByIdQueryHandler> logger)
        : IRequestHandler<GetWarehouseByIdQuery, WarehouseDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetWarehouseByIdQueryHandler> _logger = logger;

        public async Task<WarehouseDetailsDto> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Handling GetWarehouseByIdQuery for Id: {Id}", request.Id);

                var repo = _unitOfWork.GetRepository<Warehouse, Guid>();
                var wareHouse = await repo.GetAsync(new GetWareHouseByIdSpec(request.Id), cancellationToken);
                if (wareHouse is null)
                {
                    _logger.LogWarning("Warehouse not found for Id: {Id}", request.Id);
                    throw new NotFoundException<Warehouse>(request.Id);
                }
                _logger.LogInformation("Successfully retrieved Warehouse with Id: {Id}", request.Id);

                return _mapper.Map<WarehouseDetailsDto>(wareHouse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetWarehouseByIdQuery for Id: {Id}", request.Id);
                throw;
            }
        }
    }
}
