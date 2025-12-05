using Khazen.Application.BaseSpecifications.HRModule.AdvanceSpecifications;
using Khazen.Application.DOTs.HRModule.AdvanceDtos;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AdvanceUseCases.Queries.GetById
{
    internal class GetAdvanceByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAdvanceByIdQueryHandler> logger)
        : IRequestHandler<GetAdvanceByIdQuery, AdvanceDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAdvanceByIdQueryHandler> _logger = logger;

        public async Task<AdvanceDetailsDto> Handle(GetAdvanceByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Starting GetAdvanceByIdQueryHandler for AdvanceId: {AdvanceId}", request.Id);

                var advanceRepo = _unitOfWork.GetRepository<Advance, int>();
                var advance = await advanceRepo.GetAsync(
                    new GetAdvanceByIdSpecification(request.Id),
                    cancellationToken
                );

                if (advance is null)
                {
                    _logger.LogWarning("Advance record not found for Id: {AdvanceId}", request.Id);
                    throw new NotFoundException<Advance>(request.Id);
                }

                _logger.LogInformation("Successfully retrieved advance record for AdvanceId: {AdvanceId}, EmployeeId: {EmployeeId}",
                    request.Id, advance.EmployeeId);

                return _mapper.Map<AdvanceDetailsDto>(advance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving advance record with Id: {AdvanceId}", request.Id);
                throw;
            }
        }
    }
}
