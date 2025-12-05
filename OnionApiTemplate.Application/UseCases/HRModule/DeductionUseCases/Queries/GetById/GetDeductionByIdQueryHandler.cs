using Khazen.Application.BaseSpecifications.HRModule.DeductionSpecifications;
using Khazen.Application.DOTs.HRModule.DeductionDtos;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.DeductionUseCases.Queries.GetById
{
    internal class GetDeductionByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetDeductionByIdQueryHandler> logger
    ) : IRequestHandler<GetDeductionByIdQuery, DeductionDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetDeductionByIdQueryHandler> _logger = logger;

        public async Task<DeductionDetailsDto> Handle(GetDeductionByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting GetDeductionByIdQueryHandler for DeductionId: {DeductionId}", request.Id);

            try
            {
                var deductionRepo = _unitOfWork.GetRepository<Deduction, int>();

                var deduction = await deductionRepo.GetAsync(
                    new GetDeductionByIdSpecification(request.Id),
                    cancellationToken,
                    true
                );

                if (deduction is null)
                {
                    _logger.LogWarning("Deduction not found. DeductionId: {DeductionId}", request.Id);
                    throw new NotFoundException<Deduction>(request.Id);
                }

                _logger.LogInformation("Deduction found successfully. DeductionId: {DeductionId}", request.Id);

                return _mapper.Map<DeductionDetailsDto>(deduction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving DeductionId: {DeductionId}", request.Id);
                throw new ApplicationException($"An unexpected error occurred while fetching deduction {request.Id}.", ex);
            }
        }
    }
}
