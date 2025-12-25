using Khazen.Application.BaseSpecifications.HRModule.DeductionSpecifications;
using Khazen.Application.Common;
using Khazen.Application.DOTs.HRModule.Deduction;
using Khazen.Application.Specification.HRModule.DeductionSpecifications;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.DeductionUseCases.Queries.GetAll
{
    internal class GetAllDeductionsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllDeductionsQueryHandler> logger,
        IValidator<GetAllDeductionsQuery> validator
    ) : IRequestHandler<GetAllDeductionsQuery, PaginatedResult<DeductionDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllDeductionsQueryHandler> _logger = logger;
        private readonly IValidator<GetAllDeductionsQuery> _validator = validator;

        public async Task<PaginatedResult<DeductionDto>> Handle(GetAllDeductionsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Fetching paginated deductions.");

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var deductionRepo = _unitOfWork.GetRepository<Deduction, Guid>();

            var dataSpec = new GetAllDeductionsSpecification(request.QueryParameters);
            var deductions = await deductionRepo.GetAllAsync(dataSpec, cancellationToken, asNoTracking: true);

            var countSpec = new GetAllDeductionsCountSpecification();
            var totalCount = await deductionRepo.GetCountAsync(countSpec, cancellationToken, asNoTracking: true);

            var mappedData = _mapper.Map<IEnumerable<DeductionDto>>(deductions);

            _logger.LogInformation("Successfully retrieved page {PageIndex} for deductions.", request.QueryParameters.PageIndex);

            return new PaginatedResult<DeductionDto>(
                request.QueryParameters.PageIndex,
                request.QueryParameters.PageSize,
                totalCount,
                mappedData);
        }
    }
}