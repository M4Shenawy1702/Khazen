using Khazen.Application.BaseSpecifications.HRModule.SalarySepcifications;
using Khazen.Application.Common;
using Khazen.Application.Specification.HRModule.SalarySepcifications;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetAll
{
    internal class GetAllSalariesQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllSalariesQueryHandler> logger)
        : IRequestHandler<GetAllSalariesQuery, PaginatedResult<SalaryDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllSalariesQueryHandler> _logger = logger;

        public async Task<PaginatedResult<SalaryDto>> Handle(GetAllSalariesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting GetAllSalariesQuery with PageIndex: {PageIndex}, PageSize: {PageSize}, EmployeeId: {EmployeeId}, From: {From}, To: {To}",
                request.QueryParameters.PageIndex,
                request.QueryParameters.PageSize,
                request.QueryParameters.EmployeeId,
                request.QueryParameters.From,
                request.QueryParameters.To);

            var repo = _unitOfWork.GetRepository<Salary, Guid>();

            var salaries = await repo.GetAllAsync(
                new GetAllSalariesSpecification(request.QueryParameters),
                cancellationToken, true);

            var count = await repo.GetCountAsync(
                new GetAllSalariesCountSpecification(request.QueryParameters),
                cancellationToken);

            _logger.LogDebug("Retrieved {Count} salary records from the database.", count);

            var salaryDtos = _mapper.Map<List<SalaryDto>>(salaries);

            var result = new PaginatedResult<SalaryDto>(
                request.QueryParameters.PageIndex,
                request.QueryParameters.PageSize,
                count,
                salaryDtos);

            _logger.LogInformation("Returning {Count} salaries for page {PageIndex}.", salaryDtos.Count, request.QueryParameters.PageIndex);

            return result;
        }
    }
}
