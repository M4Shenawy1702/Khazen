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
            _logger.LogDebug("Fetching deduction details for Id: {DeductionId}", request.Id);

            var deductionRepo = _unitOfWork.GetRepository<Deduction, Guid>();

            var deduction = await deductionRepo.GetAsync(
                new GetDeductionByIdSpecification(request.Id),
                cancellationToken,
                asNoTracking: true
            );

            if (deduction is null)
            {
                _logger.LogWarning("Deduction with Id {DeductionId} was not found.", request.Id);
                throw new NotFoundException<Deduction>(request.Id);
            }

            return _mapper.Map<DeductionDetailsDto>(deduction);
        }
    }
}