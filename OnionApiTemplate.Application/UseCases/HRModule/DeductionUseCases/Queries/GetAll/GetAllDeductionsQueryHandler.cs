using Khazen.Application.BaseSpecifications.HRModule.DeductionSpecifications;
using Khazen.Application.DOTs.HRModule.Deduction;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.DeductionUseCases.Queries.GetAll
{
    internal class GetAllDeductionsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllDeductionsQueryHandler> logger,
        IValidator<GetAllDeductionsQuery> validator
    ) : IRequestHandler<GetAllDeductionsQuery, IEnumerable<DeductionDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllDeductionsQueryHandler> _logger = logger;
        private readonly IValidator<GetAllDeductionsQuery> _validator = validator;

        public async Task<IEnumerable<DeductionDto>> Handle(GetAllDeductionsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting GetAllDeductionsQueryHandler");
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }
            try
            {
                var deductionRepo = _unitOfWork.GetRepository<Deduction, int>();

                var deductions = await deductionRepo.GetAllAsync(
                    new GetAllDeductionsSpecification(request.QueryParameters),
                    cancellationToken,
                    true
                );

                _logger.LogInformation("Fetched {Count} deductions successfully", deductions.Count());

                return _mapper.Map<IEnumerable<DeductionDto>>(deductions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all deductions");
                throw new ApplicationException("An unexpected error occurred while retrieving deductions.", ex);
            }
        }
    }
}
